using System.Security.Claims;
using gestao_hospitalar.Application.Handlers.Pacientes;
using gestao_hospitalar.Shared;

namespace gestao_hospitalar.Api.Controllers;

public static class PacienteController
{
    public static void MapPacienteEndpoints(this WebApplication app)
    {
        app.MapPost("/paciente", async (IPacienteHandler handler, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Results.BadRequest("Você precisa estar autenticado para criar um paciente.");

            var result = await handler.CreateAsync(Guid.Parse(userId));

            if (result.Status == EStatus.Failure)
                return Results.BadRequest(result);

            return Results.Ok(result);

        }).WithTags("Paciente")
          .WithSummary("Cria um paciente para o usuário autenticado.")
          .RequireAuthorization();

        app.MapGet("/paciente/consultas/solicitadas", async (IPacienteHandler handler, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var pacienteId = Guid.Parse(userId);
            var consultas = await handler.VerConsultasSolicitadasAsync(pacienteId);

            return Results.Ok(consultas);

        }).WithTags("Paciente")
          .WithSummary("Lista todas as consultas solicitadas.")
          .RequireAuthorization();

        app.MapGet("/paciente/consultas/marcadas", async (IPacienteHandler handler, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var pacienteId = Guid.Parse(userId);
            var consultas = await handler.VerConsultasMarcadasAsync(pacienteId);

            return Results.Ok(consultas);

        }).WithTags("Paciente")
          .WithSummary("Lista todas as consultas marcadas.")
          .RequireAuthorization();

        app.MapGet("/paciente/consultas/concluidas", async (IPacienteHandler handler, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var pacienteId = Guid.Parse(userId);
            var consultas = await handler.VerConsultasConcluidasAsync(pacienteId);

            return Results.Ok(consultas);

        }).WithTags("Paciente")
          .WithSummary("Lista todas as consultas concluídas.")
          .RequireAuthorization();

        app.MapGet("/paciente/consultas/canceladas", async (IPacienteHandler handler, HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var pacienteId = Guid.Parse(userId);
            var consultas = await handler.VerConsultasCanceladasAsync(pacienteId);

            return Results.Ok(consultas);

        }).WithTags("Paciente")
          .WithSummary("Lista todas as consultas canceladas.")
          .RequireAuthorization();
    }
}
