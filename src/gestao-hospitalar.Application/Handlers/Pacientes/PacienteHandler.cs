using gestao_hospitalar.Application.Dtos.Consultas;
using gestao_hospitalar.Application.Dtos.Pacientes;
using gestao_hospitalar.Application.Mappers;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Domain.Pacientes.Aggregates;
using gestao_hospitalar.Domain.Pacientes.Repositories;
using gestao_hospitalar.Domain.Users.Repositories;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Pacientes;

public class PacienteHandler : IPacienteHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPacienteRepository _pacienteRepository;
    private readonly IUserRepository _userRepository;

    public PacienteHandler(
        IUnitOfWork unitOfWork, 
        IPacienteRepository pacienteRepository, 
        IUserRepository userRepository)
    {
        _unitOfWork = unitOfWork;
        _pacienteRepository = pacienteRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<PacienteDto>> CreateAsync(Guid userId)
    {
        var pacienteExistente = await _pacienteRepository.GetByUserIdAsync(userId);
        if (pacienteExistente != null)
            return Result<PacienteDto>.Failure("Paciente já cadastrado para este usuário.");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result<PacienteDto>.Failure("Usuário não encontrado.");

        var pacienteResult = Paciente.Criar(userId);
        if (pacienteResult.Status == EStatus.Failure)
            return Result<PacienteDto>.Failure(pacienteResult.Mensagem!);

        await _pacienteRepository.AddAsync(pacienteResult.Data!);
        await _unitOfWork.CommitAsync();

        var dto = pacienteResult.Data!.EntityToDto();

        return Result<PacienteDto>.Success(dto);
    }

    public async Task<List<ConsultaDto>> VerConsultasSolicitadasAsync(Guid pacienteId)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        if (paciente == null) return new List<ConsultaDto>();

        var consultas = paciente.VerConsultasSolicitadas();
        return consultas.Select(c => c.EntityToDto()).ToList();
    }

    public async Task<List<ConsultaDto>> VerConsultasMarcadasAsync(Guid pacienteId)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        if (paciente == null) return new List<ConsultaDto>();

        var consultas = paciente.VerConsultasMarcadas();
        return consultas.Select(c => c.EntityToDto()).ToList();
    }

    public async Task<List<ConsultaDto>> VerConsultasConcluidasAsync(Guid pacienteId)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        if (paciente == null) return new List<ConsultaDto>();

        var consultas = paciente.VerConsultasConcluidas();
        return consultas.Select(c => c.EntityToDto()).ToList();
    }

    public async Task<List<ConsultaDto>> VerConsultasCanceladasAsync(Guid pacienteId)
    {
        var paciente = await _pacienteRepository.GetByIdAsync(pacienteId);
        if (paciente == null) return new List<ConsultaDto>();

        var consultas = paciente.VerConsultasCanceladas();
        return consultas.Select(c => c.EntityToDto()).ToList();
    }
}
