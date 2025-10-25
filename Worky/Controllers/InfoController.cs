using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Worky.Services;
using Microsoft.Extensions.Logging;

namespace Worky.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class InfoController : Controller
    {
        private readonly IInfoService _infoService;
        private readonly ILogger<InfoController> _logger;

        public InfoController(IInfoService infoService, ILogger<InfoController> logger)
        {
            _infoService = infoService;
            _logger = logger;
        }

        [HttpGet("Education")]
        public async Task<IActionResult> GetEducation()
        {
            try
            {
                var edu = await _infoService.GetEducationsAsync();
                return Ok(new { education = edu });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetEducation");
                return BadRequest(500);
            }
        }

        [HttpGet("Filter")]
        public async Task<IActionResult> GetFilter()
        {
            try
            {
                var fil = await _infoService.GetFiltersAsync();
                return Ok(new { filters = fil });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetFilter");
                return BadRequest(500);
            }
        }
    }
}