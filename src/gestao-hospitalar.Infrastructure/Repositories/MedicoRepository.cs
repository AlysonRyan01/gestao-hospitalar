using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Medicos.Repositories;
using gestao_hospitalar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace gestao_hospitalar.Infrastructure.Repositories;

public class MedicoRepository : IMedicoRepository
{
    private readonly ApplicationDbContext _context;

    public MedicoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Medico?> GetByIdAsync(Guid medicoId)
        => await _context.Medicos.Include(c => c.Consultas).FirstOrDefaultAsync(m => m.Id == medicoId);

    public async Task<List<Medico>> GetAllAsync()
        => await _context.Medicos.AsNoTracking().ToListAsync();

    public async Task AddAsync(Medico medico)
    {
        _context.Medicos.Add(medico);
    }

    public async Task UpdateAsync(Medico medico)
    {
        _context.Medicos.Update(medico);
    }

    public async Task DeleteAsync(Medico medico)
    {
        _context.Medicos.Remove(medico);
    }
}