namespace GestaoAcademica.Application.Relatorios;

// Classe que espelha a seção "Relatorio" do appsettings.json.
// O ASP.NET lê o JSON e preenche estas propriedades automaticamente.
public sealed class RelatorioOptions
{
    // Nome da seção no appsettings.json
    public const string Secao = "Relatorio";

    // Valor padrão, usado se a configuração não existir
    public decimal MediaMinimaAprovacao { get; init; } = 6.0m;
}