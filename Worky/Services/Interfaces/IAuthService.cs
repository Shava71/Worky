using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Worky.Contracts;
using Worky.Models;

namespace Worky.Services;

public interface IAuthService
{
    Task<IActionResult> RegisterAsync(RegisterRequestContract request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    // Task<object> GetClaimsAsync(string userId);
}