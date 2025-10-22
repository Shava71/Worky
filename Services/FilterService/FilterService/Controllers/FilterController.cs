using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FilterService.Models;
using FilterService.Service;

namespace FilterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilterController : Controller
{
    private readonly ILogger<FilterController> _logger;
    private readonly IFilterService _filterService;

    public FilterController(ILogger<FilterController> logger, IFilterService filterService)
    {
        _logger = logger;
        _filterService = filterService;
    }

    [HttpGet("GetFilters")]
    public async Task<IActionResult> GetFilters([FromQuery] List<int> filterIds)
    {
        try
        {
            List<TypeOfActivity> filtersList = await _filterService.GetFiltersAsync(filterIds) as List<TypeOfActivity>;
            return Ok(filtersList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(500);
        }
    }

    [HttpPost("AddFilter")]
    public async Task<IActionResult> AddFilterAsync(TypeOfActivity filter)
    {
        try
        {
            int result = await _filterService.AddFilterAsync(filter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return BadRequest(500);
        }
    }
}