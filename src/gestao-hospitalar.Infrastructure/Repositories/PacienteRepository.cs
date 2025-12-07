using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Repositories;
using gestao_hospitalar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace gestao_hospitalar.Infrastructure.Repositories;

public class PacienteRepository : IPacienteRepository
{
    private readonly ApplicationDbContext _context;

    public PacienteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Paciente?> GetByIdAsync(Guid pacienteId)
        => await _context.Pacientes.Include(c => c.Consultas).FirstOrDefaultAsync(p => p.Id == pacienteId);

    public async Task<Paciente?> GetByUserIdAsync(Guid userId)
        => await _context.Pacientes.Include(c => c.Consultas).AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task<List<Paciente>> GetAllAsync()
        => await _context.Pacientes.AsNoTracking().ToListAsync();

    public async Task AddAsync(Paciente paciente)
    {
        _context.Pacientes.Add(paciente);
    }

    public async Task UpdateAsync(Paciente paciente)
    {
        _context.Pacientes.Update(paciente);
    }

    public async Task DeleteAsync(Paciente paciente)
    {
        _context.Pacientes.Remove(paciente);
    }
}