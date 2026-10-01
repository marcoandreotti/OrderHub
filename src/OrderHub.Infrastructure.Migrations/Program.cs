using Microsoft.EntityFrameworkCore;
using OrderHub.Infrastructure.Migrations;

try
{
    await using var context = new OrderHubDbContextFactory().CreateDbContext(args);
    await context.Database.MigrateAsync();
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Database migration failed: {exception.Message}");
    Environment.ExitCode = 1;
}
