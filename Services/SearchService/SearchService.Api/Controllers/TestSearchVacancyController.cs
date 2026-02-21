/*using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SearchService.DAL.Dto;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestSearchVacancyController : ControllerBase
{
    private readonly IVacancyElasticRepository _vacancyElasticRepository;
    private static List<string> defaultRelevantIds = new List<string>
    {
        "05e721d3-8126-455b-8ab7-ce1fef25af78",
        "4d045d1d-f48f-4945-9cdf-20f2b9734650",
        "f8154581-9af7-4b1e-90d6-a1be63ebe76f",
        "24168c99-531b-4b5d-9a8c-1817b4271521",
        "cd2239e2-6bd1-46dd-bb44-aa8c4cd3dd86",
        "4ee1af5d-2a0a-4b06-a26f-aeb47bc36b1a",
        "dda75e11-1ed5-4022-8385-7e49f40b62b1",
        "b952d9bd-1973-482d-9f0e-f8492ef6bb4c",
        "24168c99-531b-4b5d-9a8c-1817b4271521",
        "53817583-8504-4db2-bc31-f9f0901dc9a5",
        "96b25b71-6a20-46d7-9978-2c19f78ac2b6",
        "5b791492-3be6-497c-9510-12c5afadf5dc",
        "a44a209b-83ae-4ebc-9458-338c85b229bb",
        "14e87081-0f3e-4d6f-895a-99b251218b7e",
        "efa8f288-0529-4bdb-87c6-3788d2f9a60f",
        "ad6ce21e-2fd1-4fff-ad37-80d578b16e97",
        "a2792756-85d4-4f1b-b6e9-44235ea4c089",
        "f92f16e8-a5ef-4006-a751-848aa56c2ec9",
        "114557e2-415d-439c-8c19-047ba0a5dbc6",
        "1e649cd5-4931-4b74-8ae2-0ab7a5c0f32c",
        "96e54b0d-341c-47b8-9528-89f10d2f3dfc",
        "923fa9ee-282f-41fd-a7fe-e9f1642d7e9b",
        "c1fdc42d-23c1-4fd1-8008-7f96f6819019",
        "b697f897-0a18-43b3-993e-1f36c3698949",
        "a2f3302d-79d9-4e8b-bad4-0711b12b0363",
        "47c9875a-117c-4686-9966-733526e644ce",
    };

    public TestSearchVacancyController(IVacancyElasticRepository vacancyElasticRepository)
    {
        _vacancyElasticRepository = vacancyElasticRepository;
    }
    
    [HttpGet("SearchLexical")]
    public async Task<IActionResult> SearchLexical([FromQuery] string? aisearch,
        [FromQuery] List<string>? relevantIds,
        [FromQuery] int k = 10
        )
    {
        if (relevantIds.Count == 0)
        {
            relevantIds = defaultRelevantIds;
        }
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchLexicalAsync(aisearch);
  
        sw.Stop();

        // var response = new
        // {
        //     timeMs = sw.ElapsedMilliseconds,
        //     count = result.Count,
        //     result = result
        // };
        // return Ok(response);
        return Ok(BuildResponse(aisearch ?? "", sw.ElapsedMilliseconds, result, relevantIds, k));

    }
    
    [HttpGet("SearchSemantic")]
    public async Task<IActionResult> SearchSemantic([FromQuery] string? aisearch,
        [FromQuery] List<string>? relevantIds,
        [FromQuery] int k = 10
        )
    {
        if (relevantIds.Count == 0)
        {
            relevantIds = defaultRelevantIds;
        }
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchSemanticAsync(aisearch);
  
        sw.Stop();

        // var response = new
        // {
        //     timeMs = sw.ElapsedMilliseconds,
        //     count = result.Count,
        //     result = result
        // };
        // return Ok(response);
        return Ok(BuildResponse(aisearch ?? "", sw.ElapsedMilliseconds, result, relevantIds, k));

    }
    
    [HttpGet("SearchHybrid")]
    public async Task<IActionResult> SearchHybrid([FromQuery] string? aisearch,
        [FromQuery] List<string>? relevantIds,
        [FromQuery] int k = 10
        )
    {
        if (relevantIds.Count == 0)
        {
            relevantIds = defaultRelevantIds;
        }
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchHybridAsync(aisearch);
        sw.Stop();

        // var response = new
        // {
        //     timeMs = sw.ElapsedMilliseconds,
        //     count = result.Count,
        //     result = result
        // };
        // return Ok(response);
        return Ok(BuildResponse(aisearch ?? "", sw.ElapsedMilliseconds, result, relevantIds, k));
    }
    
    [HttpGet("SearchTwoStage")]
    public async Task<IActionResult> SearchTwoStage([FromQuery] string? aisearch, 
        [FromQuery] List<string>? relevantIds,
        [FromQuery] int k = 10)
    {
        if (relevantIds.Count == 0)
        {
            relevantIds = defaultRelevantIds;
        }
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchTwoStageAsync(aisearch);
        sw.Stop();

        // var response = new
        // {
        //     timeMs = sw.ElapsedMilliseconds,
        //     count = result.Count,
        //     result = result
        // };
        // return Ok(response);
        return Ok(BuildResponse(aisearch ?? "", sw.ElapsedMilliseconds, result, relevantIds, k));

    }
    
    private object BuildResponse(string query, long timeMs, IReadOnlyCollection<VacancySearchResultDto> results, List<string>? relevantIds, int k)
    {
        var relevant = relevantIds != null 
            ? new HashSet<string>(relevantIds) 
            : new HashSet<string>();

        var topK = results.Take(k).ToList();

        return new
        {
            query,
            timeMs,
            count = results.Count,
            k,
            precision = PrecisionAtK(topK, relevant),
            recall = RecallAtK(topK, relevant),
            ndcg = NdcgAtK(topK, relevant),
            result = results
        };
    }
    
    private double PrecisionAtK(
        List<VacancySearchResultDto> results,
        HashSet<string> relevant)
    {
        int relevantFound = results
            .Count(r => relevant.Contains(r.Document.id.ToString()));

        return (double)relevantFound / results.Count;
    }
    
    private double RecallAtK(
        List<VacancySearchResultDto> results,
        HashSet<string> relevant)
    {
        int relevantFound = results
            .Count(r => relevant.Contains(r.Document.id.ToString()));

        return (double)relevantFound / relevant.Count;
    }
    
    private double NdcgAtK(
        List<VacancySearchResultDto> results,
        HashSet<string> relevant)
    {
        double dcg = 0;

        for (int i = 0; i < results.Count; i++)
        {
            int rel = relevant.Contains(results[i].Document.id.ToString()) ? 1 : 0;

            dcg += (Math.Pow(2, rel) - 1) /
                   Math.Log2(i + 2); // позиция = i+1
        }

        // IDCG: все релевантные в начале
        int R = relevant.Count;
        double idcg = 0;

        for (int i = 0; i < Math.Min(R, results.Count); i++)
        {
            idcg += 1 / Math.Log2(i + 2);
        }

        if (idcg == 0) return 0;
        return dcg / idcg;
    }
}*/