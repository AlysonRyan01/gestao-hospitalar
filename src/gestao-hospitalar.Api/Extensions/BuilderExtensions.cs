using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using gestao_hospitalar.Application.Handlers.Consultas;
using gestao_hospitalar.Application.Handlers.Medicos;
using gestao_hospitalar.Application.Handlers.Pacientes;
using gestao_hospitalar.Application.Handlers.Users;
using gestao_hospitalar.Application.Services;
using gestao_hospitalar.Application.Validations.Users;
using gestao_hospitalar.Domain.Consultas.Repositories;
using gestao_hospitalar.Domain.Medicos.Repositories;
using gestao_hospitalar.Domain.Pacientes.Repositories;
using gestao_hospitalar.Domain.Users.Repositories;
using gestao_hospitalar.Infrastructure.Data;
using gestao_hospitalar.Infrastructure.Repositories;
using gestao_hospitalar.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace gestao_hospitalar.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddPostgreSql(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("SqlConnection"),
                b => b.MigrationsAssembly("gestao-hospitalar.Infrastructure"))
        );
    }
    
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        var secretKey = builder.Configuration["Jwt:SecretKey"]!;
        var key = Encoding.ASCII.GetBytes(secretKey);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();
    }
    
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserHandler, UserHandler>();
        builder.Services.AddScoped<IConsultaHandler, ConsultaHandler>();
        builder.Services.AddScoped<IMedicoHandler, MedicoHandler>();
        builder.Services.AddScoped<IPacienteHandler, PacienteHandler>();
        
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
        builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
        builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
    }

    public static void AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<CriarUserCommandValidator>();
    }
    
    public static void AddJwtService(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
    
    public static void ConfigureJsonSerializer(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
    }
    
    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Servers.Clear();
                document.Servers.Add(new OpenApiServer
                {
                    Url = "http://localhost:8080"
                });
                document.Components ??= new OpenApiComponents();
            
                document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        Description = "Insira o token JWT que é enviado no corpo da requisicao de login",
                        In = ParameterLocation.Header
                    }
                };

                document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                return Task.CompletedTask;
            });
        });
    }
    
    public static void AddCorsConfiguration(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        var policyName = configuration["Cors:PolicyName"];
        var origins = configuration["Cors:Origins"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(policyName!, policy =>
            {
                policy
                    .WithOrigins(origins!)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }
}