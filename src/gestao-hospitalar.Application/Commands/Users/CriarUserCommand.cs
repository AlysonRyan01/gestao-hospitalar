namespace gestao_hospitalar.Application.Commands.Users;

public record CriarUserCommand(string Nome, string Email, string Password, string Phone);