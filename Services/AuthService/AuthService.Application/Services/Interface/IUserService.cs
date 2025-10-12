using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;


namespace AuthService.Application.Services;

public interface IUserService
{
    Task<IActionResult> RegisterAsync(RegisterRequestContract request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    // Task<object> GetClaimsAsync(string userId);
}