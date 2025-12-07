using gestao_hospitalar.Application.Commands.Medicos;
using gestao_hospitalar.Application.Dtos.Medicos;
using gestao_hospitalar.Application.Handlers.Medicos;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Api.Controllers;

public static class MedicoController
{
    public static void MapMedicoEndpoints(this WebApplication app)
    {
        app.MapPost("/medicos", async (CriarMedicoCommand command, IMedicoHandler handler) =>
        {
            var result = await handler.CreateAsync(command);

            return result.Status == EStatus.Failure
                ? Results.BadRequest(result)
                : Results.Ok(result);

        }).WithTags("Medicos")
        .WithSummary("Cria um novo médico.")
        .WithDescription("Cria um médico com Nome, Telefone e Especialidade.")
        .Produces<Result<MedicoDto>>(StatusCodes.Status200OK)
        .Produces<Result<MedicoDto>>(StatusCodes.Status400BadRequest);
        
        app.MapPut("/medicos/{id:guid}", async (Guid id, AtualizarMedicoCommand command, IMedicoHandler handler) =>
        {
            var result = await handler.UpdateAsync(id, command);

            return result.Status == EStatus.Failure
                ? Results.BadRequest(result)
                : Results.Ok(result);

        }).WithTags("Medicos")
        .WithSummary("Atualiza um médico existente.")
        .WithDescription("Atualiza Nome, Telefone ou Especialidade do médico.")
        .Produces<Result<MedicoDto>>(StatusCodes.Status200OK)
        .Produces<Result<MedicoDto>>(StatusCodes.Status400BadRequest);

        app.MapDelete("/medicos/{id:guid}", async (Guid id, IMedicoHandler handler) =>
        {
            var result = await handler.DeleteAsync(id);

            return result.Status == EStatus.Failure
                ? Results.NotFound(result)
                : Results.Ok(result);

        }).WithTags("Medicos")
        .WithSummary("Remove um médico.")
        .WithDescription("Deleta um médico pelo ID.")
        .Produces<Result>(StatusCodes.Status200OK)
        .Produces<Result>(StatusCodes.Status404NotFound);

        app.MapGet("/medicos/{id:guid}", async (Guid id, IMedicoHandler handler) =>
        {
            var dto = await handler.GetByIdAsync(id);

            return dto is null
                ? Results.NotFound(Result.Failure("Médico não encontrado."))
                : Results.Ok(Result<MedicoDto>.Success(dto));

        }).WithTags("Medicos")
        .WithSummary("Busca um médico pelo ID.")
        .WithDescription("Retorna os dados de um médico específico.")
        .Produces<Result<MedicoDto>>(StatusCodes.Status200OK)
        .Produces<Result<MedicoDto>>(StatusCodes.Status404NotFound);

        app.MapGet("/medicos", async (IMedicoHandler handler) =>
        {
            var lista = await handler.GetAllAsync();
            return Results.Ok(Result<List<MedicoDto>>.Success(lista));

        }).WithTags("Medicos")
        .WithSummary("Lista todos os médicos.")
        .WithDescription("Retorna todos os médicos cadastrados no sistema.")
        .Produces<Result<List<MedicoDto>>>(StatusCodes.Status200OK);
    }
}
