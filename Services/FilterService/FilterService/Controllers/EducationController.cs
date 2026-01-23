using FilterService.Models;
using FilterService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace FilterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EducationController : Controller
{
    ILogger<EducationController> _logger;
    private readonly IEducationRepository _educationRepository;
    
    public EducationController(ILogger<EducationController> logger, IEducationRepository educationRepository)
    {
        _logger = logger;
        _educationRepository = educationRepository;
    }
    
    [HttpGet("GetEducation")]
    public async Task<IActionResult> GetAllEducations()
    {
        try
        {
            List<Education?> educations = await _educationRepository.GetAllEducations();
            return Ok(new
            {
                education = educations
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while get education list" + ex, ex.Message);
            return BadRequest(500);
        }
    }
}