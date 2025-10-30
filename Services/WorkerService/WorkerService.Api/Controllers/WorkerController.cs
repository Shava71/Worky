using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkerService.BLL.Services.Interfaces;
using WorkerService.DAL.Contracts;

namespace WorkerService.Api.Controllers;

[ApiController]
[Authorize(Roles = "Worker", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class WorkerController : Controller
{
    private readonly IWorkerService _workerService;
    private readonly ILogger<WorkerController> _logger;
    
    public WorkerController(ILogger<WorkerController> logger, IWorkerService workerService)
    {
        _logger = logger;
        _workerService = workerService;
    }
    
        // [AllowAnonymous]
        // [HttpGet("Resumes")]
        // public async Task<IActionResult> FilterResume([FromQuery] GetResumesRequest request)
        // {
        //     try
        //     {
        //         var resumes = await _workerService.FilterResumesAsync(request);
        //         return Ok(new { resumes });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in FilterResume");
        //         return BadRequest(500);
        //     }
        // }
        //
        // [AllowAnonymous]
        // [HttpGet("Resumes/Info")]
        // public async Task<IActionResult> GetResumeInfo([FromQuery] Guid resumeId)
        // {
        //     try
        //     {
        //         var resume = await _workerService.GetResumeInfoAsync(resumeId);
        //         return Ok(new { resume });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in GetResumeInfo");
        //         return BadRequest(500);
        //     }
        // }
        //
        // [HttpGet("MyResume")]
        // public async Task<IActionResult> GetMyResume([FromQuery] Guid? resumeId)
        // {
        //     try
        //     {
        //         string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         var resumes = await _workerService.GetMyResumesAsync(workerId, resumeId);
        //         return Ok(new { resumes });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in GetMyResume");
        //         return BadRequest(500);
        //     }
        // }

        [HttpPost("CreateResume")]
        public async Task<IActionResult> CreateResume([FromBody] CreateResume newResume)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation("Creating new resume for worker {workerId}", workerId);
                var id = await _workerService.CreateResumeAsync(newResume, workerId);
                _logger.LogInformation($"created new resume by id = {id}");
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateResume");
                return BadRequest(500);
            }
        }

        [HttpPut("UpdateResume")]
        public async Task<IActionResult> UpdateResume([FromBody] UpdateResume updatedResume)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _workerService.UpdateResumeAsync(updatedResume, workerId);
                return Ok(new { message = "Resume updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateResume");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteResume")]
        public async Task<IActionResult> DeleteResume([FromQuery] Guid resumeId)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _workerService.DeleteResumeAsync(resumeId, workerId);
                return Ok("Resume deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteResume");
                return BadRequest(500);
            }
        }

        [HttpPost("AddResumeFilter")]
        public async Task<IActionResult> AddResumeFilter([FromBody] AddFilter newFilter)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ids = await _workerService.AddResumeFilterAsync(newFilter, workerId);
                return Ok(new { id = ids });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddResumeFilter");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteResumeFilter")]
        public async Task<IActionResult> DeleteResumeFilter([FromQuery] Guid filterId)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _workerService.DeleteResumeFilterAsync(filterId, workerId);
                return Ok("Filter deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteResumeFilter");
                return BadRequest(500);
            }
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                string token = Request.Headers["Authorization"].First();
                
                var profile = await _workerService.GetProfileAsync(workerId, token);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return BadRequest(500);
            }
        }

        // [HttpGet("GetFeedback")]
        // public async Task<IActionResult> GetFeedback([FromQuery] Guid? vacancyId)
        // {
        //     try
        //     {
        //         string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         var feedbacks = await _workerService.GetFeedbacksAsync(workerId, vacancyId);
        //         return Ok(new { feedbacks });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in GetFeedback");
        //         return BadRequest(500);
        //     }
        // }
        //
        // [HttpPost("MakeFeedback")]
        // public async Task<IActionResult> MakeFeedback([FromBody] MakeFeedbackRequest request)
        // {
        //     try
        //     {
        //         string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         var id = await _workerService.MakeFeedbackAsync(request, workerId);
        //         return Ok(new { id });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in MakeFeedback");
        //         return BadRequest(500);
        //     }
        // }
        //
        // [HttpDelete("DeleteFeedback")]
        // public async Task<IActionResult> DeleteFeedback([FromQuery] ulong id)
        // {
        //     try
        //     {
        //         string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         await _workerService.DeleteFeedbackAsync(id, workerId);
        //         return Ok();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in DeleteFeedback");
        //         return BadRequest(500);
        //     }
        // }
}