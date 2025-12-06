using gestao_hospitalar.Application.Commands.Medicos;
using gestao_hospitalar.Application.Dtos.Medicos;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Medicos;

public interface IMedicoHandler
{
    Task<Result<MedicoDto>> CreateAsync(CriarMedicoCommand command);
    Task<Result<MedicoDto>> UpdateAsync(Guid medicoId,AtualizarMedicoCommand command);
    Task<Result> DeleteAsync(Guid medicoId);
    Task<MedicoDto?> GetByIdAsync(Guid medicoId);
    Task<List<MedicoDto>> GetAllAsync();
}