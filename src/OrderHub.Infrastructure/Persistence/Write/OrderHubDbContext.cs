using Microsoft.EntityFrameworkCore;

namespace OrderHub.Infrastructure.Persistence.Write;

/// <summary>
/// Representa o contexto do banco de dados para operações de escrita no OrderHub, utilizando o Entity Framework Core.
/// </summary>
/// <param name="options"></param>
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
    public DbSet<OrderHub.Domain.Ordering.Order> Orders => Set<OrderHub.Domain.Ordering.Order>();
    public DbSet<OrderHub.Domain.Promotions.Coupon> Coupons => Set<OrderHub.Domain.Promotions.Coupon>();
    public DbSet<OrderHub.Domain.Payments.PaymentMethod> PaymentMethods => Set<OrderHub.Domain.Payments.PaymentMethod>();
    public DbSet<OrderHub.Domain.Payments.Payment> Payments => Set<OrderHub.Domain.Payments.Payment>();
    public DbSet<OrderHub.Domain.Payments.PaymentIdempotency> PaymentIdempotencies => Set<OrderHub.Domain.Payments.PaymentIdempotency>();
    public DbSet<OrderHub.Domain.Ordering.PublicOrderRequest> PublicOrderRequests => Set<OrderHub.Domain.Ordering.PublicOrderRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderHubDbContext).Assembly);
        modelBuilder.ApplyTenantIndexes();
        base.OnModelCreating(modelBuilder);
    }
}