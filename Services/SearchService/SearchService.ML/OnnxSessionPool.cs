using System.Collections.Concurrent;
using Microsoft.ML.OnnxRuntime;

namespace SearchService.ML;

public class OnnxSessionPool
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ConcurrentQueue<InferenceSession> _pool;

    public OnnxSessionPool(string modelPath, int size)
    {
        _semaphore = new SemaphoreSlim(size, size);
        _pool = new ConcurrentQueue<InferenceSession>();

        for (int i = 0; i < size; i++)
            _pool.Enqueue(new InferenceSession(modelPath));
    }

    public async Task<InferenceSession> RentAsync()
    {
        await _semaphore.WaitAsync();
        _pool.TryDequeue(out var session);
        return session!;
    }

    public void Return(InferenceSession session)
    {
        _pool.Enqueue(session);
        _semaphore.Release();
    }
}