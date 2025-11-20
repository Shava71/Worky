using Microsoft.AspNetCore.Mvc;
using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
using SearchService.DAL.DTO;
using SearchService.DAL.Entities;

namespace SearchService.Api.Controllers;

[ApiController]
[Route("api/resumes")]
public class ResumeController : ControllerBase
{
    private readonly IResumeSearchService _searchService;

    public ResumeController(IResumeSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] GetResumesRequest request)
    {
        var result = await _searchService.SearchAsync(request);
        return Ok(result);
    }
}