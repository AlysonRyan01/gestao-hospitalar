using FluentValidation;
using gestao_hospitalar.Application.Commands.Medicos;
using gestao_hospitalar.Application.Dtos.Medicos;
using gestao_hospitalar.Application.Mappers;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Domain.Medicos.Aggregates;
using gestao_hospitalar.Domain.Medicos.Repositories;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Medicos;

public class MedicoHandler : IMedicoHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IValidator<CriarMedicoCommand> _criarMedicoValidator;
    private readonly IValidator<AtualizarMedicoCommand> _atualizarMedicoValidator;

    public MedicoHandler(
        IUnitOfWork unitOfWork, 
        IMedicoRepository medicoRepository, 
        IValidator<CriarMedicoCommand> criarMedicoValidator,
        IValidator<AtualizarMedicoCommand> atualizarMedicoValidator)
    {
        _unitOfWork = unitOfWork;
        _medicoRepository = medicoRepository;
        _criarMedicoValidator = criarMedicoValidator;
        _atualizarMedicoValidator = atualizarMedicoValidator;
    }

    public async Task<Result<MedicoDto>> CreateAsync(CriarMedicoCommand command)
    {
        var validationResult = _criarMedicoValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result<MedicoDto>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var resultado = Medico.Criar(command.Nome, command.Telefone, command.Especialidade);
        if (resultado.Status == EStatus.Failure)
            return Result<MedicoDto>.Failure(resultado.Mensagem!);

        await _medicoRepository.AddAsync(resultado.Data!);
        await _unitOfWork.CommitAsync();

        return Result<MedicoDto>.Success(resultado.Data!.EntityToDto());
    }

    public async Task<Result<MedicoDto>> UpdateAsync(Guid medicoId, AtualizarMedicoCommand command)
    {
        if (medicoId == Guid.Empty)
            return Result<MedicoDto>.Failure("O ID do médico é obrigatório");
        
        var validationResult = _atualizarMedicoValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result<MedicoDto>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var medico = await _medicoRepository.GetByIdAsync(medicoId);
        if (medico == null)
            return Result<MedicoDto>.Failure("Médico não encontrado.");

        var resultado = medico.Atualizar(command.Nome, command.Telefone, command.Especialidade);
        if (resultado.Status == EStatus.Failure)
            return Result<MedicoDto>.Failure(resultado.Mensagem!);

        await _medicoRepository.UpdateAsync(medico);
        await _unitOfWork.CommitAsync();

        return Result<MedicoDto>.Success(medico.EntityToDto());
    }

    public async Task<Result> DeleteAsync(Guid medicoId)
    {
        var medico = await _medicoRepository.GetByIdAsync(medicoId);
        if (medico == null)
            return Result.Failure("Médico não encontrado.");

        await _medicoRepository.DeleteAsync(medico);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<MedicoDto?> GetByIdAsync(Guid medicoId)
    {
        var medico = await _medicoRepository.GetByIdAsync(medicoId);
        return medico?.EntityToDto();
    }

    public async Task<List<MedicoDto>> GetAllAsync()
    {
        var medicos = await _medicoRepository.GetAllAsync();
        return medicos.Select(m => m.EntityToDto()).ToList();
    }
}