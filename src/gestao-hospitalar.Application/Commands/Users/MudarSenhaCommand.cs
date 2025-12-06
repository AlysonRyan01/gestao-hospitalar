namespace gestao_hospitalar.Application.Commands.Users;

public record MudarSenhaCommand(string SenhaAtual, string NovaSenha);