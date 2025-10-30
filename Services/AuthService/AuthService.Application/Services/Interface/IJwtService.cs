
namespace AuthService.Application.Services;

public interface IJwtService
{
    public string GenerateToken(Guid userId, IList<string> Role);
}