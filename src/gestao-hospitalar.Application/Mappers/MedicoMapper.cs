using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Medicos;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Domain.Medicos.Aggregates;

namespace gestao_hospitalar.Application.Mappers;

public static class MedicoMapper
{
    public static MedicoDto EntityToDto(this Medico medico)
        => new MedicoDto(
            medico.Id,
            medico.Nome,
            medico.Telefone,
            medico.Especialidade,
            medico.Consultas.Select(c => new ConsultaDto(
                    c.Id,
                    c.Sobre,
                    c.MarcadoPara,
                    c.FinalConsultaPara,
                    c.MotivoCancelamento,
                    c.Status,
                    null,
                    null
                ))
                .ToList());
}