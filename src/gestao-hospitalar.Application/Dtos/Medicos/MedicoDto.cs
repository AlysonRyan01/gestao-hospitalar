using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Domain.Consultas.Aggregates;

namespace gestao_hospitalar.Application.Dtos.Medicos;

public record MedicoDto(Guid Id, string Nome, string Telefone, string Especialidade, List<ConsultaDto> Consultas);