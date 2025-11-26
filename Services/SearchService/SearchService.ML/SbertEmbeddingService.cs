using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Tokenizers.HuggingFace.Tokenizer;

namespace SearchService.ML;

public class SbertEmbeddingService : IDisposable
{
    private readonly InferenceSession _session;
    private readonly Tokenizer _tokenizer;

    private int _dim;  // Динамическая размерность эмбеддинга

    public SbertEmbeddingService()
    {
        var opts = new SessionOptions
        {
            LogSeverityLevel = OrtLoggingLevel.ORT_LOGGING_LEVEL_ERROR,
            GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL,
            IntraOpNumThreads = Environment.ProcessorCount
        };

        // === Загружаем модель ===
        _session = new InferenceSession("model/model.onnx", opts);

        // Определяем размерность выхода (например, 384)
        var outputMeta = _session.OutputMetadata.First().Value;
        _dim = outputMeta.Dimensions.Last(); // Output: [1,128,384], берём 384

        // === Загружаем токенизатор ===
        _tokenizer = Tokenizer.FromFile("model/tokenizer.json");
    }

    public float[] GetEmbedding(string text)
    {
        const int MaxLen = 128;

        if (string.IsNullOrWhiteSpace(text))
            return new float[_dim];

        // --- Tokenization ---
        var enc = _tokenizer.Encode("query: "+text, addSpecialTokens: true).First();

        // --- Truncate ---
        var ids = enc.Ids.Take(MaxLen).Select(i => (long)i).ToArray();
        var mask = enc.AttentionMask.Take(MaxLen).Select(i => (long)i).ToArray();

        // --- Pad to MaxLen ---
        Array.Resize(ref ids, MaxLen);
        Array.Resize(ref mask, MaxLen);

        // token_type_ids = zeros
        var types = new long[MaxLen];

        // --- Create tensors ---
        var inputIds = new DenseTensor<long>(ids, new[] { 1, MaxLen });
        var attentionMask = new DenseTensor<long>(mask, new[] { 1, MaxLen });
        var tokenTypeIds = new DenseTensor<long>(types, new[] { 1, MaxLen });

        var inputs = new List<NamedOnnxValue>
        {
            NamedOnnxValue.CreateFromTensor("input_ids", inputIds),
            NamedOnnxValue.CreateFromTensor("attention_mask", attentionMask),
            NamedOnnxValue.CreateFromTensor("token_type_ids", tokenTypeIds)
        };

        // --- Run inference ---
        using var result = _session.Run(inputs);

        // --- Extract embedding ---
        var raw = result.First().AsEnumerable<float>().ToArray();

        // Если модель вернула [1,384] — просто вернуть
        if (raw.Length == _dim)
            return raw;

        // Если модель вернула [1,128,384] — взять CLS (первый токен)
        if (raw.Length == MaxLen * _dim)
        {
            var cls = new float[_dim];
            Array.Copy(raw, 0, cls, 0, _dim);
            return cls;
        }

        // fallback: pad до нужной размерности
        return Pad(raw, _dim);
    }

    private float[] Pad(float[] v, int dim)
    {
        var r = new float[dim];
        Array.Copy(v, 0, r, 0, Math.Min(v.Length, dim));
        return r;
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}