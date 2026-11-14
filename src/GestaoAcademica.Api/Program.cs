using GestaoAcademica.Api.ErrorHandling;
using GestaoAcademica.Application;
using GestaoAcademica.Infrastructure;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.

builder.Services.AddControllers();

// AddProblemDetails: faz o ASP.NET usar o formato ProblemDetails também nos erros que ele mesmo gera (400, 404 de rota)
builder.Services.AddProblemDetails();

// Registra o nosso tratador global de exceções
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("A connection string 'DefaultConnection' não foi configurada.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Precisa vir primeiro, para capturar exceções de tudo o que vem depois
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI: interface no navegador que lê o /openapi/v1.json e permite testar os endpoints
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Gestão Acadêmica API v1");
        options.RoutePrefix = "swagger";   // disponível em /swagger
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
