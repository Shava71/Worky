using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Security.Claims;
using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CompanyService.Api.Controllers
{
    [Authorize(Roles = "Company", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController : Controller
    {
        private readonly ICompnayService _companyService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompnayService companyService, ILogger<CompanyController> logger,  ICompanyRepository companyRepository)
        {
            _companyService = companyService;
            _logger = logger;
            _companyRepository = companyRepository;
        }
        
        [AllowAnonymous]
        [HttpGet("Vacancies/Info")]
        public async Task<IActionResult> GetVacancyInfo([FromQuery] Guid vacancyId)
        {
            try
            {
                VacancyDtos vacancy = await _companyService.GetVacancyInfoAsync(vacancyId);
                return Ok(new { vacancy });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVacancyInfo");
                return BadRequest(500);
            }
        }


        [HttpGet("MyVacancy")]
        public async Task<IActionResult> GetMyVacancy([FromQuery] Guid? vacancyId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                IEnumerable<VacancyDtos> vacancies = await _companyService.GetMyVacanciesAsync(Guid.Parse(companyId), vacancyId);
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
                Guid id = await _companyService.CreateVacancyAsync(newVacancy, companyId);
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
        public async Task<IActionResult> DeleteVacancy([FromQuery] Guid vacancyId)
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
        public async Task<IActionResult> DeleteVacancyFilter([FromQuery] Guid filterId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _companyService.DeleteVacancyFilterAsync(filterId, companyId);
                return Ok("Filter deleted");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Vacancy key not found + {0}", filterId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteVacancyFilter");
                return BadRequest(500);
            }
        }

        // [HttpGet("Statistics/json")]
        // public async Task<IActionResult> GetCompanyStatisticsJson([FromQuery] int start_year, [FromQuery] int start_month, [FromQuery] int end_year, [FromQuery] int end_month)
        // {
        //     try
        //     {
        //         string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         var stats = await _companyService.GetStatisticsJsonAsync(companyId, start_year, start_month, end_year, end_month);
        //         return Ok(stats);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in GetCompanyStatisticsJson");
        //         return BadRequest(500);
        //     }
        // }
        //
        // [HttpGet("Statistics/pdf")]
        // public async Task<IActionResult> GetCompanyStatisticsPdf([FromQuery] int start_year, [FromQuery] int start_month, [FromQuery] int end_year, [FromQuery] int end_month)
        // {
        //     try
        //     {
        //         string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         var pdf = await _companyService.GetStatisticsPdfAsync(companyId, start_year, start_month, end_year, end_month);
        //         return File(pdf, "application/pdf", "statistics.pdf");
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error in GetCompanyStatisticsPdf");
        //         return BadRequest(500);
        //     }
        // }

        [HttpGet("Flyer")]
        public async Task<IActionResult> GetFlyer([FromQuery] Guid vacancyId, string url)
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
                string authHeader = Request.Headers["Authorization"].First();
                string token = authHeader.Replace("Bearer ", "");
                CompanyProfileDtos profile = await _companyService.GetProfileAsync(companyId, token);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfile");
                return BadRequest(500);
            }
        }
        
        [HttpPut("Update/{userId}")]
        public async Task<IActionResult> UpdateCompany(Guid userId, [FromBody] UpdateCompanyDto dto)
        {
            var company = await _companyRepository.GetCompanyByIdAsync(userId);

            if (company == null)
                return NotFound("Company not found");

            company.name = dto.Name;
            company.email = dto.Email;
            company.phoneNumber = dto.PhoneNumber;
            company.latitude = dto.Latitude;
            company.longitude = dto.Longitude;
            company.website = dto.Website;

            await _companyRepository.UpdateCompanyAsync(company);

            return Ok(company);
        }
    }
}