using gestao_hospitalar.Api.Controllers;
using gestao_hospitalar.Api.Middlewares;
using gestao_hospitalar.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace gestao_hospitalar.Api.Extensions;

public static class AppExtensions
{
    public static void AddAuthorization(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
    
    public static void AddEndpoints(this WebApplication app)
    {
        app.MapUserEndpoints();
        app.MapMedicoEndpoints();
        app.MapConsultaEndpoints();
        app.MapPacienteEndpoints();
    }
    
    public static void AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("http://localhost:8080/openapi/v1.json", "gestao_hospitalar v1");
                options.RoutePrefix = string.Empty; 
            });
        }
    }
    
    public static void AddMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }
    }
    
    public static void AddExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
    }
}