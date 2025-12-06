using gestao_hospitalar.Application.Commands.Consultas;
using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Consultas;

public interface IConsultaHandler
{
    Task<Result<ConsultaDto>> SolicitarAgendamentoAsync(Guid pacienteId, SolicitarAgendamentoCommand command);
    Task<Result<ConsultaDto>> AgendarConsultaAsync(Guid consultaId, Guid medicoId, AgendarConsultaCommand command);
    Task<Result> ConcluirConsultaAsync(Guid consultaId);
    Task<Result> CancelarConsultaAsync(Guid consultaId, CancelarConsultaCommand command);
    Task<List<ConsultaDto>> VerConsultasPorPacienteAsync(Guid pacienteId);
    Task<List<ConsultaDto>> VerConsultasPorMedicoAsync(Guid medicoId);
    Task<ConsultaDto?> GetByIdAsync(Guid consultaId);
}