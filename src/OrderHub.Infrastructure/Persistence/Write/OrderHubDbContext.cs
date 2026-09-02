using Microsoft.EntityFrameworkCore;

namespace OrderHub.Infrastructure.Persistence.Write;

public sealed class OrderHubDbContext(DbContextOptions<OrderHubDbContext> options) : DbContext(options)
{
    public DbSet<OrderHub.Domain.Tenancy.Tenant> Tenants => Set<OrderHub.Domain.Tenancy.Tenant>();
    public DbSet<OrderHub.Domain.Tenancy.Establishment> Establishments => Set<OrderHub.Domain.Tenancy.Establishment>();
    public DbSet<OrderHub.Domain.Identity.AdministrativeUser> AdministrativeUsers => Set<OrderHub.Domain.Identity.AdministrativeUser>();
    public DbSet<OrderHub.Domain.Operations.ServiceTable> ServiceTables => Set<OrderHub.Domain.Operations.ServiceTable>();
    public DbSet<OrderHub.Domain.Operations.BusinessHours> BusinessHours => Set<OrderHub.Domain.Operations.BusinessHours>();
    public DbSet<OrderHub.Domain.Catalog.Category> Categories => Set<OrderHub.Domain.Catalog.Category>();
    public DbSet<OrderHub.Domain.Catalog.Product> Products => Set<OrderHub.Domain.Catalog.Product>();
    public DbSet<OrderHub.Domain.Catalog.Additional> Additionals => Set<OrderHub.Domain.Catalog.Additional>();
    public DbSet<OrderHub.Domain.Catalog.AdditionalGroup> AdditionalGroups => Set<OrderHub.Domain.Catalog.AdditionalGroup>();
    public DbSet<OrderHub.Domain.Customers.Customer> Customers => Set<OrderHub.Domain.Customers.Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderHubDbContext).Assembly);
        modelBuilder.ApplyTenantIndexes();
        base.OnModelCreating(modelBuilder);
    }
}
