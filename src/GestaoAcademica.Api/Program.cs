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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
