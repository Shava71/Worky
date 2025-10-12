using AuthService.Application.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestContract registerRequest)
    {
        return await _userService.RegisterAsync(registerRequest);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var response = await _userService.LoginAsync(loginRequest);
        if (response == null) return Unauthorized();
        return Ok(response);
    }
    
}