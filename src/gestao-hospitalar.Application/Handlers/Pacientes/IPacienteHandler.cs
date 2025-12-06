using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Pacientes;

public interface IPacienteHandler
{
    Task<Result<PacienteDto>> CreateAsync(Guid userId);
    Task<List<ConsultaDto>> VerConsultasSolicitadasAsync(Guid pacienteId);
    Task<List<ConsultaDto>> VerConsultasMarcadasAsync(Guid pacienteId);
    Task<List<ConsultaDto>> VerConsultasConcluidasAsync(Guid pacienteId);
    Task<List<ConsultaDto>> VerConsultasCanceladasAsync(Guid pacienteId);
}