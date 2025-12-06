using gestao_hospitalar.Domain.Pacientes.Aggregates;

namespace gestao_hospitalar.Domain.Pacientes.Repositories;

public interface IPacienteRepository
{
    Task<Paciente?> GetByIdAsync(Guid pacienteId);
    Task<Paciente?> GetByUserIdAsync(Guid userId);
    Task<List<Paciente>> GetAllAsync();
    Task AddAsync(Paciente paciente);
    Task UpdateAsync(Paciente paciente);
    Task DeleteAsync(Paciente paciente);
}