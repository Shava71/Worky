using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Worky.Contracts;
using Worky.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;

namespace Worky.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class AuthorizationController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(IAuthService authService, ILogger<AuthorizationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestContract registerRequest)
        {
            return await _authService.RegisterAsync(registerRequest);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);
            if (response == null) return Unauthorized();
            return Ok(response);
        }

        // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        // [HttpGet("Claims")]
        // public async Task<IActionResult> GetClaims()
        // {
        //     try
        //     {
        //         var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //         var claims = await _authService.GetClaimsAsync(id);
        //         return Ok(claims);
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "failed to get claims");
        //         return BadRequest(500);
        //     }
        // }
    }
}