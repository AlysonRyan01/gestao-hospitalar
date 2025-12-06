using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Consultas.Enums;

namespace gestao_hospitalar.Domain.Consultas.Repositories;

public interface IConsultaRepository
{
    Task<Consulta?> GetByIdAsync(Guid consultaId);
    Task<List<Consulta>> GetByPacienteIdAsync(Guid pacienteId);
    Task<List<Consulta>> GetByMedicoIdAsync(Guid medicoId);
    Task<List<Consulta>> GetByPacienteIdAndStatusAsync(Guid pacienteId, EStatusConsulta status);
    Task<List<Consulta>> GetByMedicoIdAndStatusAsync(Guid medicoId, EStatusConsulta status);
    Task AddAsync(Consulta consulta);
    Task UpdateAsync(Consulta consulta);
    Task DeleteAsync(Consulta consulta);
}