
using CompanyService.DAL.Clients;

namespace CompanyService.BLL.Services.Http.Interfaces;

public interface IAuthClient
{
    Task<UserResponse?> GetUserByIdAsync(string userId, string token, CancellationToken cancellationToken = default);
}