using System.Data.Common;

namespace OrderHub.Application.Abstractions.Persistence;

public interface IReadConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default);
}
