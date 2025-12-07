using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Domain.Users.Aggregates;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace gestao_hospitalar.Infrastructure.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;

    public AuthenticationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string> Generate(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var secret = _configuration["Jwt:SecretKey"];

        if (string.IsNullOrWhiteSpace(secret))
            throw new Exception("O SecretKey da API não pode estar vazia");

        var key = Encoding.ASCII.GetBytes(secret);

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = await GenerateClaims(user),
            SigningCredentials = credentials,
            Expires = DateTime.UtcNow.AddHours(14),

        };

        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);

    }

    private Task<ClaimsIdentity> GenerateClaims(User user)
    {
        var ci = new ClaimsIdentity();
        
        ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        ci.AddClaim(new Claim(ClaimTypes.Name, user.Name));
        ci.AddClaim(new Claim(ClaimTypes.Email, user.Email));
        
        return Task.FromResult(ci);
    }

    public async Task<bool> ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;
        
        var secret = _configuration["Jwt:SecretKey"]!;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(secret);

        try
        {
            await Task.Run(() =>
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            });

            return true;
        }
        catch
        {
            return false;
        }
    }
}