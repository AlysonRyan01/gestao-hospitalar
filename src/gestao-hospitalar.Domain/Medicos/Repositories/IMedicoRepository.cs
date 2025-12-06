using gestao_hospitalar.Domain.Medicos.Aggregates;

namespace gestao_hospitalar.Domain.Medicos.Repositories;

public interface IMedicoRepository
{
    Task<Medico?> GetByIdAsync(Guid medicoId);
    Task<List<Medico>> GetAllAsync();
    Task AddAsync(Medico medico);
    Task UpdateAsync(Medico medico);
    Task DeleteAsync(Medico medico);
}