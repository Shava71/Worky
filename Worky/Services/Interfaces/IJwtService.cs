using Worky.Models;

namespace Worky.Services;

public interface IJwtService
{
    public string GenerateToken(Guid userId, IList<string> Role);
}