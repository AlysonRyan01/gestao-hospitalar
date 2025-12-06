using gestao_hospitalar.Application.Commands.Users;
using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Users;

public interface IUserHandler
{
    Task<Result<UserDto>> RegisterAsync(CriarUserCommand command);
    Task<Result<UserDto>> LoginAsync(LoginCommand command);
    Task<Result> ChangePasswordAsync(Guid userId, MudarSenhaCommand command);
    Task<UserDto?> GetByIdAsync(Guid userId);
    Task<List<UserDto>> GetAllAsync();
}