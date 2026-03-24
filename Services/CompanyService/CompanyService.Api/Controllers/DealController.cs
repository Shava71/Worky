// <copyright file="DealController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Security.Claims;
using CompanyService.BLL.Services.Implementations;
using CompanyService.BLL.Services.Interfaces;
using CompanyService.DAL.Contracts;
using CompanyService.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyService.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Company", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class DealController : Controller
{
    private readonly ILogger<DealController> logger;
    private readonly IDealService dealService;

    public DealController(ILogger<DealController> logger, IDealService dealService)
    {
        this.logger = logger;
        this.dealService = dealService;
    }

    [AllowAnonymous]
    [HttpGet("Tarrif")]
    public async Task<IActionResult> GetTarrif([FromQuery] int? tariffId)
    {
        try
        {
            // var tarrifs = _dbContext.Tarrifs.ToList();
            //
            // if (tariffId.HasValue)
            // {
            //     tarrifs = tarrifs.Where(t => t.id == tariffId).ToList();
            // }
            // return Ok(new { tarrifs = tarrifs});
            if (tariffId.HasValue)
            {
                Tarrif? tarrif = await dealService.GetTariff(tariffId);
                return Ok(new { tarrif = tarrif });
            }
            else
            {
                List<Tarrif>? tarrifs = await dealService.GetTariff();
                return Ok(new { tarrifs = tarrifs });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while get tarrifs by company");
            return BadRequest(500);
        }
    }

    [HttpPost("MakeDeal")]
    public async Task<IActionResult> MakeDeal(MakeDealRequest request)
    {
        try
        {
            // DateTime dateTime = DateTime.UtcNow.Date;
            // DateOnly curDate = DateOnly.FromDateTime(dateTime);
            // int sum = _dbContext.Tarrifs.Where(t => t.id == request.tarrif_id).Select(t => t.price).FirstOrDefault();
            //
            // Guid currentIdUser = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            // Deal newDeal = new Deal()
            // {
            //     tariff_id = request.tarrif_id,
            //     company_id = currentIdUser.ToString(),
            //     status = true, //пока ставим статус оплаченного
            //     date_start = curDate,
            //     date_end = curDate.AddMonths(request.countMonth),
            //     sum = sum * request.countMonth
            // };
            // await _dbContext.Deals.AddAsync(newDeal);
            // await _dbContext.SaveChangesAsync();
            // return Ok(new { id = newDeal.id});
            string companyId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            Guid id = await dealService.CreateDeal(request, Guid.Parse(companyId));
            return Ok(new { id = id });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occured while make deal by company");
            return BadRequest(500);
        }
    }
}