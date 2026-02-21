using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SearchService.Contract;
using SearchService.DAL;

namespace SearchService.Api.Controllers;

[ApiController]
[Route("api/search")]
public class SearchFeedbackController : ControllerBase
{
    private readonly SearchDbContext _context;

    public SearchFeedbackController(SearchDbContext context)
    {
        _context = context;
    }

    [HttpPost("click")]
    public async Task<IActionResult> Click([FromBody] ClickRequest request)
    {
        var impression = await _context.SearchImpressions
            .FirstOrDefaultAsync(x =>
                x.SessionId == request.SessionId &&
                x.DocumentId == request.DocumentId);

        if (impression == null)
            return NotFound();

        impression.Clicked = true;
        impression.DwellTimeMs = request.DwellTimeMs;

        await _context.SaveChangesAsync();

        return Ok();
    }
}