using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Consultas.Enums;
using gestao_hospitalar.Domain.Consultas.Repositories;
using gestao_hospitalar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace gestao_hospitalar.Infrastructure.Repositories;

public class ConsultaRepository : IConsultaRepository
{
    private readonly ApplicationDbContext _context;

    public ConsultaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Consulta?> GetByIdAsync(Guid consultaId)
        => await _context.Consultas
            .Include(c => c.Paciente)
                .ThenInclude(p => p.User)
            .Include(c => c.Medico)
            .FirstOrDefaultAsync(c => c.Id == consultaId);

    public async Task<List<Consulta>> GetByPacienteIdAsync(Guid pacienteId)
        => await _context.Consultas
            .Where(c => c.PacienteId == pacienteId)
            .Include(c => c.Medico)
            .ToListAsync();

    public async Task<List<Consulta>> GetByMedicoIdAsync(Guid medicoId)
        => await _context.Consultas
            .Where(c => c.MedicoId == medicoId)
            .Include(c => c.Paciente)
                .ThenInclude(p => p.User)
            .ToListAsync();

    public async Task<List<Consulta>> GetByPacienteIdAndStatusAsync(Guid pacienteId, EStatusConsulta status)
        => await _context.Consultas
            .Where(c => c.PacienteId == pacienteId && c.Status == status)
            .Include(c => c.Medico)
            .ToListAsync();

    public async Task<List<Consulta>> GetByMedicoIdAndStatusAsync(Guid medicoId, EStatusConsulta status)
        => await _context.Consultas
            .Where(c => c.MedicoId == medicoId && c.Status == status)
            .Include(c => c.Paciente)
                .ThenInclude(p => p.User)
            .ToListAsync();

    public async Task AddAsync(Consulta consulta)
    {
        _context.Consultas.Add(consulta);
    }

    public async Task UpdateAsync(Consulta consulta)
    {
        _context.Consultas.Update(consulta);
    }

    public async Task DeleteAsync(Consulta consulta)
    {
        _context.Consultas.Remove(consulta);
    }
}