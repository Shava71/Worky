using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Worky.Services;
using Worky.Contracts;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Worky.DTO;

namespace Worky.Controllers
{
    [Authorize(Roles = "Company", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController : Controller
    {
        private readonly ICompnayService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompnayService companyService, ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet("Resumes")]
        public async Task<IActionResult> FilterResume([FromQuery] GetResumesRequest request)
        {
            try
            {
                var resumes = await _companyService.FilterResumesAsync(request);
                return Ok(new { resumes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FilterResume");
                return BadRequest(500);
            }
        }

        [AllowAnonymous]
        [HttpGet("Resumes/Info")]
        public async Task<IActionResult> GetResumeInfo([FromQuery] ulong resumeId)
        {
            try
            {
                var resume = await _companyService.GetResumeInfoAsync(resumeId);
                return Ok(new { resume });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetResumeInfo");
                return BadRequest(500);
            }
        }

        [HttpGet("MyVacancy")]
        public async Task<IActionResult> GetMyVacancy([FromQuery] ulong? vacancyId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var vacancies = await _companyService.GetMyVacanciesAsync(companyId, vacancyId);
                return Ok(new { vacancies });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMyVacancy");
                return BadRequest(500);
            }
        }

        [HttpPost("CreateVacancy")]
        public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancy newVacancy)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var id = await _companyService.CreateVacancyAsync(newVacancy, companyId);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateVacancy");
                return BadRequest(500);
            }
        }

        [HttpPut("UpdateVacancy")]
        public async Task<IActionResult> UpdateVacancy([FromBody] UpdateVacancy updatedVacancy)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _companyService.UpdateVacancyAsync(updatedVacancy, companyId);
                return Ok(new { message = "Vacancy updated" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateVacancy");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteVacancy")]
        public async Task<IActionResult> DeleteVacancy([FromQuery] ulong vacancyId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _companyService.DeleteVacancyAsync(vacancyId, companyId);
                return Ok("Vacancy deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteVacancy");
                return BadRequest(500);
            }
        }

        [HttpPost("AddVacancyFilter")]
        public async Task<IActionResult> AddVacancyFilter([FromBody] AddFilter newFilter)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var ids = await _companyService.AddVacancyFilterAsync(newFilter, companyId);
                return Ok(new { id = ids });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddVacancyFilter");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteVacancyFilter")]
        public async Task<IActionResult> DeleteVacancyFilter([FromQuery] ulong filterId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _companyService.DeleteVacancyFilterAsync(filterId, companyId);
                return Ok("Filter deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteVacancyFilter");
                return BadRequest(500);
            }
        }

        [HttpGet("GetFeedback")]
        public async Task<IActionResult> GetFeedback([FromQuery] ulong? resumeId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var feedbacks = await _companyService.GetFeedbacksAsync(companyId, resumeId);
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
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var id = await _companyService.MakeFeedbackAsync(request, companyId);
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
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _companyService.DeleteFeedbackAsync(id, companyId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteFeedback");
                return BadRequest(500);
            }
        }

        [HttpGet("Statistics/json")]
        public async Task<IActionResult> GetCompanyStatisticsJson([FromQuery] int start_year, [FromQuery] int start_month, [FromQuery] int end_year, [FromQuery] int end_month)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var stats = await _companyService.GetStatisticsJsonAsync(companyId, start_year, start_month, end_year, end_month);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCompanyStatisticsJson");
                return BadRequest(500);
            }
        }

        [HttpGet("Statistics/pdf")]
        public async Task<IActionResult> GetCompanyStatisticsPdf([FromQuery] int start_year, [FromQuery] int start_month, [FromQuery] int end_year, [FromQuery] int end_month)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var pdf = await _companyService.GetStatisticsPdfAsync(companyId, start_year, start_month, end_year, end_month);
                return File(pdf, "application/pdf", "statistics.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCompanyStatisticsPdf");
                return BadRequest(500);
            }
        }

        [HttpGet("Flyer")]
        public async Task<IActionResult> GetFlyer([FromQuery] ulong vacancyId, string url)
        {
            try
            {
                var flyer = await _companyService.GetFlyerAsync(vacancyId,url);
                return File(flyer, "application/pdf", $"flyer_{vacancyId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFlyer");
                return BadRequest(500);
            }
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var profile = await _companyService.GetProfileAsync(companyId);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return BadRequest(500);
            }
        }
    }
}