using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Domain.Pacientes.Aggregates;

namespace gestao_hospitalar.Application.Mappers;

public static class PacienteMapper
{
    public static PacienteDto EntityToDto(this Paciente paciente)
        => new PacienteDto(paciente.Id,
            new UserDto(paciente.User.Id, paciente.User.Name, paciente.User.Email, paciente.User.Phone),
            paciente.Consultas.Select(
                c => new ConsultaDto(
                c.Id,
                c.Sobre,
                c.MarcadoPara,
                c.FinalConsultaPara,
                c.MotivoCancelamento,
                c.Status,
                c.Paciente.EntityToDto(),
                c.Medico?.EntityToDto())).ToList());
}