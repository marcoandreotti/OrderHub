namespace OrderHub.Infrastructure.Persistence;

/// <summary>
/// Representa as opções de configuração do banco de dados, incluindo a string de conexão necessária para estabelecer a comunicação com o banco de dados.
/// </summary>
public sealed class DatabaseOptions
{
    public const string SectionName = "Database";
    public string ConnectionString { get; init; }
}