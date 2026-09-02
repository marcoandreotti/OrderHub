using Microsoft.EntityFrameworkCore;
using OrderHub.Domain.SharedKernel;

namespace OrderHub.Infrastructure.Persistence.Write;

internal static class TenantModelConventions
{
    public static void ApplyTenantIndexes(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                     .Where(type => typeof(ITenantScopedEntity).IsAssignableFrom(type.ClrType)))
        {
            modelBuilder.Entity(entityType.ClrType).HasIndex(nameof(ITenantScopedEntity.TenantId));
        }
    }
}
