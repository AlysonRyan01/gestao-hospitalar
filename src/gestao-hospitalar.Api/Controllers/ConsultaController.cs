using System.Security.Claims;
using gestao_hospitalar.Application.Commands.Consultas;
using gestao_hospitalar.Application.Handlers.Consultas;
using gestao_hospitalar.Shared;
using gestao_hospitalar.Application.Dtos.Consultas;

namespace gestao_hospitalar.Api.Controllers;

public static class ConsultaController
{
    public static void MapConsultaEndpoints(this WebApplication app)
    {
        app.MapPost("/consultas/solicitar", async (
            SolicitarAgendamentoCommand command,
            IConsultaHandler handler,
            HttpContext http) =>
        {
            var pacienteId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (pacienteId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var result = await handler.SolicitarAgendamentoAsync(Guid.Parse(pacienteId), command);

            if (result.Status == EStatus.Failure)
                return Results.BadRequest(result);

            return Results.Ok(result);

        }).WithTags("Consultas")
          .WithSummary("Solicita um agendamento de consulta.")
          .WithDescription("Paciente autenticado solicita uma consulta informando o motivo e a data desejada.")
          .Produces<Result<ConsultaDto>>()
          .RequireAuthorization();
        
        app.MapPut("/consultas/{consultaId:guid}/agendar", async (
            Guid consultaId,
            AgendarConsultaCommand command,
            HttpContext http,
            IConsultaHandler handler) =>
        {
            var medicoId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (medicoId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var result = await handler.AgendarConsultaAsync(
                consultaId,
                Guid.Parse(medicoId),
                command);

            if (result.Status == EStatus.Failure)
                return Results.BadRequest(result);

            return Results.Ok(result);

        }).WithTags("Consultas")
          .WithSummary("Médico agenda uma consulta.")
          .WithDescription("O médico define horário e duração de uma consulta solicitada.")
          .Produces<Result<ConsultaDto>>()
          .RequireAuthorization();
        
        app.MapPut("/consultas/{consultaId:guid}/concluir", async (
            Guid consultaId,
            IConsultaHandler handler,
            HttpContext http) =>
        {
            var medicoId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (medicoId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var result = await handler.ConcluirConsultaAsync(consultaId);

            if (result.Status == EStatus.Failure)
                return Results.BadRequest(result);

            return Results.Ok(result);

        }).WithTags("Consultas")
          .WithSummary("Conclui uma consulta.")
          .WithDescription("O médico conclui uma consulta previamente agendada.")
          .Produces<Result>()
          .RequireAuthorization();

        app.MapPut("/consultas/{consultaId:guid}/cancelar", async (
            Guid consultaId,
            CancelarConsultaCommand command,
            IConsultaHandler handler,
            HttpContext http) =>
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var result = await handler.CancelarConsultaAsync(consultaId, command);

            if (result.Status == EStatus.Failure)
                return Results.BadRequest(result);

            return Results.Ok(result);

        }).WithTags("Consultas")
          .WithSummary("Cancela uma consulta.")
          .WithDescription("Paciente ou médico cancela a consulta com justificativa.")
          .Produces<Result>()
          .RequireAuthorization();

        app.MapGet("/consultas/paciente", async (
            IConsultaHandler handler,
            HttpContext http) =>
        {
            var pacienteId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (pacienteId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var consultas = await handler.VerConsultasPorPacienteAsync(Guid.Parse(pacienteId));

            return Results.Ok(consultas);

        }).WithTags("Consultas")
          .WithSummary("Lista consultas do paciente autenticado.")
          .RequireAuthorization();

        app.MapGet("/consultas/medico", async (
            IConsultaHandler handler,
            HttpContext http) =>
        {
            var medicoId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (medicoId is null)
                return Results.BadRequest("Você precisa estar autenticado.");

            var consultas = await handler.VerConsultasPorMedicoAsync(Guid.Parse(medicoId));

            return Results.Ok(consultas);

        }).WithTags("Consultas")
          .WithSummary("Lista consultas do médico autenticado.")
          .RequireAuthorization();

        app.MapGet("/consultas/{consultaId:guid}", async (
            Guid consultaId,
            IConsultaHandler handler) =>
        {
            var consulta = await handler.GetByIdAsync(consultaId);

            if (consulta is null)
                return Results.NotFound("Consulta não encontrada.");

            return Results.Ok(consulta);

        }).WithTags("Consultas")
          .WithSummary("Busca os detalhes de uma consulta pelo ID.");
    }
}
