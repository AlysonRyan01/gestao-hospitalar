using FluentValidation;
using gestao_hospitalar.Application.Commands.Users;
using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Application.Mappers;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Domain.Users.Aggregates;
using gestao_hospitalar.Domain.Users.Repositories;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Application.Handlers.Users;

public class UserHandler : IUserHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CriarUserCommand> _criarUserValidator;
    private readonly IValidator<LoginCommand> _logarUserValidator;
    private readonly IValidator<MudarSenhaCommand> _mudarSenhaValidator;

    public UserHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork,
        IValidator<CriarUserCommand> criarUserValidator,
        IValidator<LoginCommand> logarUserValidator,
        IValidator<MudarSenhaCommand> mudarSenhaValidator)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _criarUserValidator = criarUserValidator;
        _logarUserValidator = logarUserValidator;
        _mudarSenhaValidator = mudarSenhaValidator;
    }

    public async Task<Result<UserDto>> RegisterAsync(CriarUserCommand command)
    {
        var validationResult = _criarUserValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result<UserDto>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var existingUser = await _userRepository.GetByEmailAsync(command.Email);
        if (existingUser != null)
            return Result<UserDto>.Failure("Já existe um usuário com este e-mail.");
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(command.Password);

        var userResult = User.Criar(command.Nome, command.Email, hashedPassword, command.Phone);
        if (userResult.Status == EStatus.Failure)
            return Result<UserDto>.Failure(userResult.Mensagem!);

        await _userRepository.AddAsync(userResult.Data!);
        await _unitOfWork.CommitAsync();

        return Result<UserDto>.Success(userResult.Data!.EntityToDto());
    }

    public async Task<Result<UserDto>> LoginAsync(LoginCommand command)
    {
        var validationResult = _logarUserValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result<UserDto>.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var user = await _userRepository.GetByEmailAsync(command.Email);
        if (user == null)
            return Result<UserDto>.Failure("Usuário ou senha inválidos.");

        if (!BCrypt.Net.BCrypt.Verify(command.Password, user.Password))
            return Result<UserDto>.Failure("Usuário ou senha inválidos.");

        return Result<UserDto>.Success(user.EntityToDto());
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, MudarSenhaCommand command)
    {
        var validationResult = _mudarSenhaValidator.Validate(command);
        if (!validationResult.IsValid)
            return Result.Failure(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return Result.Failure("Usuário não encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(command.SenhaAtual, user.Password))
            return Result.Failure("Senha atual incorreta.");

        var mudarSenhaResultado = user.MudarSenha(BCrypt.Net.BCrypt.HashPassword(command.NovaSenha));
        if (mudarSenhaResultado.Status == EStatus.Failure)
            return Result.Failure(mudarSenhaResultado.Mensagem!);
        
        await _userRepository.UpdateAsync(user);
        await _unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user?.EntityToDto();
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(u => u.EntityToDto()).ToList();
    }
}