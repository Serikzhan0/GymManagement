using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GymManagementApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace GymManagementApi;

public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var key = _configuration["Jwt:Key"] ?? "SuperSecretKeyForGymManagementApiExamTesting12345!";
        var issuer = _configuration["Jwt:Issuer"] ?? "GymManagementApi";
        var audience = _configuration["Jwt:Audience"] ?? "GymManagementApi";
        var expiryMinutes = int.TryParse(_configuration["Jwt:ExpiresInMinutes"], out var minutes) ? minutes : 120;

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
