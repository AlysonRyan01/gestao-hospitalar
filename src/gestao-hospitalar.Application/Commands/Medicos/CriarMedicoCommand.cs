namespace gestao_hospitalar.Application.Commands.Medicos;

public record CriarMedicoCommand(string Nome, string Telefone, string Especialidade);