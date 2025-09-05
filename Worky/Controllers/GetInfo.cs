using Microsoft.AspNetCore.Mvc;
using Worky.Context;

namespace Worky.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GetInfo : Controller
{
    private readonly WorkyDbContext _dbContext;
    ILogger<CompanyController> _logger;

    public GetInfo(WorkyDbContext dbContext, ILogger<CompanyController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet("Education")]
    public async Task<IActionResult> GetEducation()
    {
        try
        {
            var edu = _dbContext.Educations.ToList();
            return Ok(new { education = edu });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show resumes by company");
            return BadRequest(500);
        }
    }

    [HttpGet("Filter")]
    public async Task<IActionResult> GetFilter()
    {
        try
        {
            var fil = _dbContext.typeOfActivities.OrderBy(f => f.type).ToList();
            return Ok(new { filters = fil });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while show resumes by company");
            return BadRequest(500);
        }
    }
}