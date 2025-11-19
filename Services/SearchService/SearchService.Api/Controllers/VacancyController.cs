using Microsoft.AspNetCore.Mvc;
using SeachService.DAL.DTO;
using SearchService.BLL.Services.Interfaces;
using SearchService.Contract;
using SearchService.DAL.Entities;

namespace SearchService.Api.Controllers;

[ApiController]
[Route("api/vacancies")]
public class VacancyController : ControllerBase
{
    private readonly IVacancySearchService _searchService;

    public VacancyController(IVacancySearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<VacancyDocument>>> Search([FromQuery] GetVacanciesRequest request)
    {
        var result = await _searchService.SearchAsync(request);
        return Ok(result);
    }
}