using Microsoft.EntityFrameworkCore;
using OrderHub.Infrastructure.Migrations;

await using var context = new OrderHubDbContextFactory().CreateDbContext(args);
await context.Database.MigrateAsync();
