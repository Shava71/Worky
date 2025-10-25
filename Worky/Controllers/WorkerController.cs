using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Worky.Services;
using Worky.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Worky.Controllers
{
    [Authorize(Roles = "Worker", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WorkerController : Controller
    {
        private readonly IWorkerService _workerService;
        private readonly ILogger<WorkerController> _logger;

        public WorkerController(IWorkerService workerService, ILogger<WorkerController> logger)
        {
            _workerService = workerService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("Vacancies")]
        public async Task<IActionResult> FilterVacancy([FromQuery] GetVacanciesRequest request)
        {
            try
            {
                var vacancies = await _workerService.FilterVacanciesAsync(request);
                return Ok(new { vacancies });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FilterVacancy");
                return BadRequest(500);
            }
        }

        [AllowAnonymous]
        [HttpGet("Vacancies/Info")]
        public async Task<IActionResult> GetVacancyInfo([FromQuery] ulong vacancyId)
        {
            try
            {
                var vacancy = await _workerService.GetVacancyInfoAsync(vacancyId);
                return Ok(new { vacancy });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVacancyInfo");
                return BadRequest(500);
            }
        }

        [HttpGet("MyResume")]
        public async Task<IActionResult> GetMyResume([FromQuery] ulong? resumeId)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var resumes = await _workerService.GetMyResumesAsync(workerId, resumeId);
                return Ok(new { resumes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMyResume");
                return BadRequest(500);
            }
        }

        [HttpPost("CreateResume")]
        public async Task<IActionResult> CreateResume([FromBody] CreateResume newResume)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var id = await _workerService.CreateResumeAsync(newResume, workerId);
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
        public async Task<IActionResult> DeleteResume([FromQuery] ulong resumeId)
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
        public async Task<IActionResult> DeleteResumeFilter([FromQuery] ulong filterId)
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
                var profile = await _workerService.GetProfileAsync(workerId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return BadRequest(500);
            }
        }

        [HttpGet("GetFeedback")]
        public async Task<IActionResult> GetFeedback([FromQuery] ulong? vacancyId)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var feedbacks = await _workerService.GetFeedbacksAsync(workerId, vacancyId);
                return Ok(new { feedbacks });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFeedback");
                return BadRequest(500);
            }
        }

        [HttpPost("MakeFeedback")]
        public async Task<IActionResult> MakeFeedback([FromBody] MakeFeedbackRequest request)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var id = await _workerService.MakeFeedbackAsync(request, workerId);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MakeFeedback");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteFeedback")]
        public async Task<IActionResult> DeleteFeedback([FromQuery] ulong id)
        {
            try
            {
                string workerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _workerService.DeleteFeedbackAsync(id, workerId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteFeedback");
                return BadRequest(500);
            }
        }
    }
}