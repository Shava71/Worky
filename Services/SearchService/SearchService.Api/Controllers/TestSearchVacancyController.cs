using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SearchService.DAL.Repositories.Interfaces;

namespace SearchService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestSearchVacancyController : ControllerBase
{
    private readonly IVacancyElasticRepository _vacancyElasticRepository;

    public TestSearchVacancyController(IVacancyElasticRepository vacancyElasticRepository)
    {
        _vacancyElasticRepository = vacancyElasticRepository;
    }
    
    [HttpGet("SearchLexical")]
    public async Task<IActionResult> SearchLexical([FromQuery] string? aisearch)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchLexicalAsync(aisearch);
  
        sw.Stop();

        var response = new
        {
            timeMs = sw.ElapsedMilliseconds,
            count = result.Count,
            result = result
        };
        return Ok(response);
    }
    
    [HttpGet("SearchSemantic")]
    public async Task<IActionResult> SearchSemantic([FromQuery] string? aisearch)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchSemanticAsync(aisearch);
  
        sw.Stop();

        var response = new
        {
            timeMs = sw.ElapsedMilliseconds,
            count = result.Count,
            result = result
        };
        return Ok(response);
    }
    
    [HttpGet("SearchHybrid")]
    public async Task<IActionResult> SearchHybrid([FromQuery] string? aisearch)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchHybridAsync(aisearch);
        sw.Stop();

        var response = new
        {
            timeMs = sw.ElapsedMilliseconds,
            count = result.Count,
            result = result
        };
        return Ok(response);
    }
    
    [HttpGet("SearchTwoStage")]
    public async Task<IActionResult> SearchTwoStage([FromQuery] string? aisearch)
    {
        Stopwatch sw = Stopwatch.StartNew();
        var result = await _vacancyElasticRepository.SearchTwoStageAsync(aisearch);
        sw.Stop();

        var response = new
        {
            timeMs = sw.ElapsedMilliseconds,
            count = result.Count,
            result = result
        };
        return Ok(response);
    }
}