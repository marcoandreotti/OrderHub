using System.Data.Common;
using Microsoft.Extensions.Options;
using Npgsql;
using OrderHub.Application.Abstractions.Persistence;

namespace OrderHub.Infrastructure.Persistence.Read;

public sealed class NpgsqlReadConnectionFactory(IOptions<DatabaseOptions> options) : IReadConnectionFactory
{
    private readonly string connectionString = options.Value.ConnectionString;

    public async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
