using System.Security.Claims;
using FeedbackService.BLL.Services.Interfaces;
using FeedbackService.DAL.Contracts;
using FeedbackService.DAL.Entities;
using FeedbackService.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackService.Api.Controllers;

[ApiController]
[Authorize( AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackService feedbackService;
    private readonly ILogger<FeedbackController> logger;


    public FeedbackController(IFeedbackService feedbackService, ILogger<FeedbackController> logger)
    {
        this.feedbackService = feedbackService;
        this.logger = logger;
    }
    
    [HttpPost("MakeFeedback")]
    public async Task<IActionResult> MakeFeedback([FromBody] MakeFeedbackRequest request)
    {
        try
        {
            Guid id = await feedbackService.AddFeedbackAsync(resumeId: request.resume_id, vacancyId: request.vacancy_id);
            return Ok(new {feedbackId = id});
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return BadRequest(500);
        }
       
    }
    
    [HttpDelete("DeleteFeedback")]
    public async Task<IActionResult> DeleteFeedback([FromQuery] Guid feedbackId)
    {
        try
        {
            Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Guid? id = await feedbackService.DeleteFeedbackAsync(feedbackId, currentIdUser);
            if (!id.HasValue)
            {
                return NotFound();
            }

            return Ok(new { feedbackId = id });
        }
        catch (KeyNotFoundException ex)
        {
            this.logger.LogError(ex, ex.Message);
            return BadRequest(ex);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ex.Message);
            return BadRequest(500);
        }
    }

    [HttpGet("GetFeedback")]
    public async Task<IActionResult> GetFeedback([FromQuery] Guid? Id)
    {
        Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var feedbacks = await feedbackService.GetAllFeedbacksAsync(currentIdUser, Id);

    }
}