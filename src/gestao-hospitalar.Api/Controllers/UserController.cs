using System.Security.Claims;
using gestao_hospitalar.Application.Commands.Users;
using gestao_hospitalar.Application.Dtos.Users;
using gestao_hospitalar.Application.Handlers.Users;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Api.Controllers;

public static class UserController
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapPost("/login", async (LoginCommand command, IAuthenticationService jwtService, IUserHandler handler) =>
        {
            var loginResult = await handler.LoginAsync(command);
            if (loginResult.Status == EStatus.Failure)
                return Results.BadRequest(loginResult);
            
            var token = loginResult.Data;
            if (token is null)
                return Results.BadRequest(Result<string>.Failure("Erro ao fazer login"));

            return Results.Ok(Result<string>.Success(token));
        }).WithTags("Auth")
        .WithSummary("Autentica um usuário e gera um token JWT.")
        .WithDescription("Recebe email e senha, verifica as credenciais e retorna um token JWT para autenticação.")
        .Produces<Result<string>>()
        .Produces<Result<string>>(StatusCodes.Status401Unauthorized);

        app.MapPost("/register", async (CriarUserCommand command, IUserHandler handler) =>
        {
            var criarUsuarioResultado = await handler.RegisterAsync(command);
            if (criarUsuarioResultado.Status == EStatus.Failure)
                return Results.BadRequest(criarUsuarioResultado);

            return Results.Ok(criarUsuarioResultado);
        }).WithTags("Auth")
        .WithSummary("Registra um novo usuário")
        .WithDescription("Cria uma nova conta de usuário com base no email e senha fornecidos. "
                         + "Se o usuário já existir ou ocorrer erro na criação, retorna uma mensagem de erro.")
        .Produces<Result<UserDto>>(StatusCodes.Status200OK, "application/json")
        .Produces<Result<UserDto>>(StatusCodes.Status400BadRequest, "application/json");
        
        app.MapPut("/mudar-senha", async (MudarSenhaCommand command, IUserHandler handler,  HttpContext http) =>
        {
            var userIdString = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdString, out var userId))
                return Results.Unauthorized();
            
            var mudarSenhaResultado = await handler.ChangePasswordAsync(userId, command);
            if (mudarSenhaResultado.Status == EStatus.Failure)
                return Results.BadRequest(mudarSenhaResultado);

            return Results.Ok(mudarSenhaResultado);
        }).WithTags("Auth")
        .WithSummary("Muda a senha do usuário")
        .WithDescription("Muda a senha de um usuário já autenticado")
        .Produces<Result>(StatusCodes.Status200OK, "application/json")
        .Produces<Result>(StatusCodes.Status400BadRequest, "application/json")
        .RequireAuthorization();
    }
}