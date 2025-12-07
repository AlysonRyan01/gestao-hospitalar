using FluentValidation;
using gestao_hospitalar.Application.Commands.Consultas;
using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Mappers;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Domain.Consultas.Aggregates;
using gestao_hospitalar.Domain.Consultas.Repositories;
using gestao_hospitalar.Domain.Medicos.Repositories;
using gestao_hospitalar.Domain.Pacientes.Repositories;
using gestao_hospitalar.Domain.Users.Repositories;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Consultas;

public class ConsultaHandler : IConsultaHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConsultaRepository _consultaRepository;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMedicoRepository _medicoRepository;
    private readonly IValidator<SolicitarAgendamentoCommand> _solicitarAgendamentoValidator;
    private readonly IValidator<AgendarConsultaCommand> _agendarConsultaValidator;
    private readonly IValidator<CancelarConsultaCommand> _cancelarConsultaValidator;

    public ConsultaHandler(
        IUnitOfWork unitOfWork,
        IConsultaRepository consultaRepository,
        IPacienteRepository pacienteRepository, 
        IUserRepository userRepository,
        IMedicoRepository medicoRepository,
        IValidator<SolicitarAgendamentoCommand> solicitarAgendamentoValidator,
        IValidator<AgendarConsultaCommand> agendarConsultaValidator,
        IValidator<CancelarConsultaCommand> cancelarConsultaValidator)
    {
        _unitOfWork = unitOfWork;
        _consultaRepository = consultaRepository;
        _pacienteRepository = pacienteRepository;
        _userRepository = userRepository;
        _medicoRepository = medicoRepository;
        _solicitarAgendamentoValidator = solicitarAgendamentoValidator;
        _agendarConsultaValidator = agendarConsultaValidator;
        _cancelarConsultaValidator = cancelarConsultaValidator;
    }

    public async Task<Result<ConsultaDto>> SolicitarAgendamentoAsync(
        Guid pacienteId, 
        SolicitarAgendamentoCommand command)
    {
        if (pacienteId == Guid.Empty)
            return Result<ConsultaDto>.Failure("O ID do paciente é obrigatório");
        
        var validationResult = _solicitarAgendamentoValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result<ConsultaDto>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var paciente = await _pacienteRepository.GetByUserIdAsync(pacienteId);
        if (paciente == null)
            return Result<ConsultaDto>.Failure("Paciente não encontrado.");

        var resultado = Consulta.SolicitarAgendamento(paciente.Id, command.Sobre, command.MarcarPara);
        if (resultado.Status == EStatus.Failure)
            return Result<ConsultaDto>.Failure(resultado.Mensagem!);

        await _consultaRepository.AddAsync(resultado.Data!);
        await _unitOfWork.CommitAsync();

        return Result<ConsultaDto>.Success(resultado.Data!.EntityToDto());
    }

    public async Task<Result<ConsultaDto>> AgendarConsultaAsync(
        Guid consultaId, 
        Guid medicoId, 
        AgendarConsultaCommand command)
    {
        if (consultaId == Guid.Empty)
            return Result<ConsultaDto>.Failure("O ID da consulta é obrigatória");

        if (medicoId == Guid.Empty)
            return Result<ConsultaDto>.Failure("O ID do médico é obrigatório");
        
        var validationResult = _agendarConsultaValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result<ConsultaDto>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var consulta = await _consultaRepository.GetByIdAsync(consultaId);
        if (consulta == null)
            return Result<ConsultaDto>.Failure("Consulta não encontrada.");

        var medico = await _medicoRepository.GetByIdAsync(medicoId);
        if (medico == null)
            return Result<ConsultaDto>.Failure("Médico não encontrado.");

        var medicoTemHoraLivre = medico.ValidarHorarioDisponivel(consulta, command.DuracaoEmMinutos);
        if (medicoTemHoraLivre.Status == EStatus.Failure)
            return Result<ConsultaDto>.Failure(medicoTemHoraLivre.Mensagem!);

        var resultado = consulta.AgendarConsulta(medicoId, command.DuracaoEmMinutos);
        if (resultado.Status == EStatus.Failure)
            return Result<ConsultaDto>.Failure(resultado.Mensagem!);

        await _consultaRepository.UpdateAsync(consulta);
        await _unitOfWork.CommitAsync();

        return Result<ConsultaDto>.Success(consulta.EntityToDto());
    }

    public async Task<Result> ConcluirConsultaAsync(Guid consultaId)
    {
        var consulta = await _consultaRepository.GetByIdAsync(consultaId);
        if (consulta == null)
            return Result.Failure("Consulta não encontrada.");

        var resultado = consulta.ConcluirConsulta();
        if (resultado.Status == EStatus.Failure)
            return Result.Failure(resultado.Mensagem!);

        await _consultaRepository.UpdateAsync(consulta);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<Result> CancelarConsultaAsync(Guid consultaId, CancelarConsultaCommand command)
    {
        if (consultaId == Guid.Empty)
            return Result.Failure("O ID da consulta é obrigatório");
        
        var validationResult = _cancelarConsultaValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var consulta = await _consultaRepository.GetByIdAsync(consultaId);
        if (consulta == null)
            return Result.Failure("Consulta não encontrada.");

        var resultado = consulta.CancelarConsulta(command.MotivoCancelamento);
        if (resultado.Status == EStatus.Failure)
            return Result.Failure(resultado.Mensagem!);

        await _consultaRepository.UpdateAsync(consulta);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<List<ConsultaDto>> VerConsultasPorPacienteAsync(Guid pacienteId)
    {
        var paciente = await _pacienteRepository.GetByUserIdAsync(pacienteId);
        if (paciente == null)
            return new List<ConsultaDto>();
        
        var consultas = await _consultaRepository.GetByPacienteIdAsync(paciente.Id);
        return consultas.Select(c => c.EntityToDto()).ToList();
    }

    public async Task<List<ConsultaDto>> VerConsultasPorMedicoAsync(Guid medicoId)
    {
        var consultas = await _consultaRepository.GetByMedicoIdAsync(medicoId);
        return consultas.Select(c => c.EntityToDto()).ToList();
    }

    public async Task<ConsultaDto?> GetByIdAsync(Guid consultaId)
    {
        var consulta = await _consultaRepository.GetByIdAsync(consultaId);
        return consulta?.EntityToDto();
    }
}
