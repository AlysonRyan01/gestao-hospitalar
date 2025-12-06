using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Users;

namespace gestao_hospitalar.Application.Dtos.Pacientes;

public record PacienteDto(Guid Id, UserDto User, List<ConsultaDto> Consultas);