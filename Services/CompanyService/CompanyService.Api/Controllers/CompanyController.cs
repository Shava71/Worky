// <copyright file="CompanyController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Claims;
using System.Threading.Tasks;
using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.DTO;
using CompanyService.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CompanyService.Api.Controllers
{
    [Authorize(Roles = "Company", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CompanyController : Controller
    {
        private readonly ICompnayService companyService;
        private readonly ICompanyRepository companyRepository;
        private readonly ILogger<CompanyController> logger;

        public CompanyController(ICompnayService companyService, ILogger<CompanyController> logger,  ICompanyRepository companyRepository)
        {
            this.companyService = companyService;
            this.logger = logger;
            this.companyRepository = companyRepository;
        }

        [AllowAnonymous]
        [HttpGet("Vacancies/Info")]
        public async Task<IActionResult> GetVacancyInfo([FromQuery] Guid vacancyId)
        {
            try
            {
                VacancyDtos vacancy = await companyService.GetVacancyInfoAsync(vacancyId);
                return Ok(new { vacancy });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetVacancyInfo");
                return BadRequest(500);
            }
        }

        [HttpGet("MyVacancy")]
        public async Task<IActionResult> GetMyVacancy([FromQuery] Guid? vacancyId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                IEnumerable<VacancyDtos> vacancies = await companyService.GetMyVacanciesAsync(Guid.Parse(companyId), vacancyId);
                return Ok(new { vacancies });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetMyVacancy");
                return BadRequest(500);
            }
        }

        [HttpPost("CreateVacancy")]
        public async Task<IActionResult> CreateVacancy([FromBody] CreateVacancy newVacancy)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                Guid id = await companyService.CreateVacancyAsync(newVacancy, companyId);
                return Ok(new { id });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in CreateVacancy");
                return BadRequest(500);
            }
        }

        [HttpPut("UpdateVacancy")]
        public async Task<IActionResult> UpdateVacancy([FromBody] UpdateVacancy updatedVacancy)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await companyService.UpdateVacancyAsync(updatedVacancy, companyId);
                return Ok(new { message = "Vacancy updated" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in UpdateVacancy");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteVacancy")]
        public async Task<IActionResult> DeleteVacancy([FromQuery] Guid vacancyId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await companyService.DeleteVacancyAsync(vacancyId, companyId);
                return Ok("Vacancy deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in DeleteVacancy");
                return BadRequest(500);
            }
        }

        [HttpPost("AddVacancyFilter")]
        public async Task<IActionResult> AddVacancyFilter([FromBody] AddFilter newFilter)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var ids = await companyService.AddVacancyFilterAsync(newFilter, companyId);
                return Ok(new { id = ids });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AddVacancyFilter");
                return BadRequest(500);
            }
        }

        [HttpDelete("DeleteVacancyFilter")]
        public async Task<IActionResult> DeleteVacancyFilter([FromQuery] Guid filterId)
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await companyService.DeleteVacancyFilterAsync(filterId, companyId);
                return Ok("Filter deleted");
            }
            catch (KeyNotFoundException ex)
            {
                logger.LogError(ex, "Vacancy key not found + {0}", filterId);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in DeleteVacancyFilter");
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
                var flyer = await companyService.GetFlyerAsync(vacancyId,url);
                return File(flyer, "application/pdf", $"flyer_{vacancyId}.pdf");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetFlyer");
                return BadRequest(500);
            }
        }

        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!; // добавили !
                string authHeader = Request.Headers["Authorization"].First()!; // добавили !
                string token = authHeader.Replace("Bearer ", string.Empty); // заменили "" на string.Empty
                CompanyProfileDtos profile = await companyService.GetProfileAsync(companyId, token);
                return Ok(profile);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetProfile");
                return BadRequest(500);
            }
        }

        [HttpPut("Update/{userId}")]
        public async Task<IActionResult> UpdateCompany(Guid userId, [FromBody] UpdateCompanyDto dto)
        {
            var company = await companyRepository.GetCompanyByIdAsync(userId);

            if (company == null)
            {
                return NotFound("Company not found");
            }

            company.name = dto.Name;
            company.email = dto.Email;
            company.phoneNumber = dto.PhoneNumber;
            company.latitude = dto.Latitude;
            company.longitude = dto.Longitude;
            company.website = dto.Website;

            await companyRepository.UpdateCompanyAsync(company);

            return Ok(company);
        }
    }
}