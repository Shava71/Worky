using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FilterService.Models;

namespace FilterService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilterController : Controller
{
    private readonly ILogger<FilterController> _logger;

    public FilterController(ILogger<FilterController> logger)
    {
        _logger = logger;
    }

   
}