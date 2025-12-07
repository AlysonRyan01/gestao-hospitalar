using gestao_hospitalar.Domain.Users.Aggregates;

namespace gestao_hospitalar.Application.Services;

public interface IAuthenticationService
{
    Task<string> Generate(User user);
    Task<bool> ValidateToken(string token);
}