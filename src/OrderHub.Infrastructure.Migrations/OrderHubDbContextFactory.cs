using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using OrderHub.Infrastructure.Persistence.Write;

namespace OrderHub.Infrastructure.Migrations;

public sealed class OrderHubDbContextFactory : IDesignTimeDbContextFactory<OrderHubDbContext>
{
    public OrderHubDbContext CreateDbContext(string[] args)
    {
        var connectionString = GetConnectionString(args);
        var options = new DbContextOptionsBuilder<OrderHubDbContext>()
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsAssembly(typeof(OrderHubDbContextFactory).Assembly.FullName))
            .Options;

        return new OrderHubDbContext(options);
    }

    private static string GetConnectionString(string[] args)
    {
        var index = Array.IndexOf(args, "--connection");
        if (index < 0 || index == args.Length - 1 || string.IsNullOrWhiteSpace(args[index + 1]))
        {
            throw new InvalidOperationException("Pass the PostgreSQL connection string after --connection.");
        }

        return args[index + 1];
    }
}
