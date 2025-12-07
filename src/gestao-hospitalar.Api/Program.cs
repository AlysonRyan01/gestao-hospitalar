using gestao_hospitalar.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddCorsConfiguration();
builder.AddFluentValidation();
builder.AddPostgreSql();
builder.AddAuthentication();
builder.AddJwtService();
builder.ConfigureJsonSerializer();
builder.AddDependencies();
builder.AddSwagger();

var app = builder.Build();
app.AddExceptionMiddleware();
app.UseCors(builder.Configuration["Cors:PolicyName"]!);
Console.WriteLine(builder.Configuration["Cors:PolicyName"]!);
Console.WriteLine(builder.Configuration["Cors:Origins"]!);
app.AddMigrations();
app.AddAuthorization();
app.AddEndpoints();
app.AddSwagger();

app.Run();