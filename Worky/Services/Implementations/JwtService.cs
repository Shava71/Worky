using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Worky.Context;
using Worky.Models;

namespace Worky.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _key;
    private readonly string? _issuer;
    private readonly string? _audience;

    public JwtService(IConfiguration config, SymmetricSecurityKey securityKey)
    {
        _configuration = config;
        _key = securityKey;
        _issuer = _configuration["Jwt:Issuer"];
        _audience = _configuration["Jwt:Audience"];
    }

    // public string GenerateToken(Guid userId, int Role)
    // {
    //     var issuer = _configuration["Jwt:Issuer"];
    //     var audience = _configuration["Jwt:Audience"];
    //     var key = _configuration["Jwt:Key"];
    //     var tokenLifetimeMins = _configuration.GetValue<int>("Jwt:TokenLifetimeMins");
    //     var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenLifetimeMins);
    //
    //     var tokenDescriptor = new SecurityTokenDescriptor
    //     {
    //         Subject = new ClaimsIdentity(new[]
    //         {
    //             new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
    //             new Claim(ClaimTypes.Role, Role.ToString()),
    //         }),
    //         Expires = tokenExpiryTimeStamp,
    //         Issuer = issuer,
    //         Audience = audience,
    //         SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    //             SecurityAlgorithms.HmacSha256Signature)
    //
    //     };
    //
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var securityToken = tokenHandler.CreateToken(tokenDescriptor);
    //     var token = tokenHandler.WriteToken(securityToken);
    //     
    //     return token;
    // }

    public string GenerateToken(Guid userId, IList<string> Roles)
    {
        var tokenLifetimeMins = _configuration.GetValue<int>("Jwt:ExpiresInMinutes");

        Claim[] claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };
        claims = claims.Concat(Roles.Select(role => new Claim(ClaimTypes.Role, role))).ToArray();

        var signingCredentials = new SigningCredentials(
            _key, SecurityAlgorithms.HmacSha256);

        var JwtToken = new JwtSecurityToken
        (
            // Subject = new ClaimsIdentity(new[]
            // {
            //     new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            // }),
            // NotBefore = now,
            // IssuedAt = now,
            // Expires = expires,
            // Issuer = issuer,
            // Audience = audience,
            // signingCredentials = new SigningCredentials(
            //     new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            //     SecurityAlgorithms.HmacSha256Signature)
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenLifetimeMins),
            signingCredentials: signingCredentials
        );

        // tokenDescriptor.Subject.AddClaims(Roles.Select(r => new Claim(ClaimTypes.Role, r)));

        return new JwtSecurityTokenHandler().WriteToken(JwtToken);
    }
}