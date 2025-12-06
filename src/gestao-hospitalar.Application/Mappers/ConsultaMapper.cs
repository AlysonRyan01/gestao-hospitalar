using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Medicos;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Domain.Consultas.Aggregates;

namespace gestao_hospitalar.Application.Mappers;

public static class ConsultaMapper
{
    public static ConsultaDto EntityToDto(this Consulta consulta)
        => new ConsultaDto(
            consulta.Id,
            consulta.Sobre,
            consulta.MarcadoPara,
            consulta.FinalConsultaPara,
            consulta.MotivoCancelamento,
            consulta.Status,
            new PacienteDto(
                consulta.Paciente.Id,
                new UserDto(
                    consulta.Paciente.User.Id, 
                    consulta.Paciente.User.Name, 
                    consulta.Paciente.User.Email, 
                    consulta.Paciente.User.Phone),
                new List<ConsultaDto>()
            ),
            consulta.Medico == null ? null : new MedicoDto(
                consulta.Medico.Id,
                consulta.Medico.Nome,
                consulta.Medico.Telefone,
                consulta.Medico.Especialidade,
                new List<ConsultaDto>()
            ));
}