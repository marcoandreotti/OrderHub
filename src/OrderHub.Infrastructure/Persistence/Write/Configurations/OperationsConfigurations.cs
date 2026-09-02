using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderHub.Domain.Operations;

namespace OrderHub.Infrastructure.Persistence.Write.Configurations;

internal sealed class ServiceTableConfiguration : IEntityTypeConfiguration<ServiceTable>
{
    public void Configure(EntityTypeBuilder<ServiceTable> builder)
    {
        builder.ToTable("service_table", DatabaseSchemas.Operations);
        builder.HasKey(table => table.Id); builder.Property(table => table.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(table => table.TenantId).HasColumnName("tenant_id"); builder.Property(table => table.EstablishmentId).HasColumnName("establishment_id");
        builder.Property(table => table.Code).HasColumnName("code").HasMaxLength(30); builder.Property(table => table.Description).HasColumnName("description").HasMaxLength(100);
        builder.Property(table => table.QrCodeToken).HasColumnName("qr_code_token").HasMaxLength(100); builder.Property(table => table.IsActive).HasColumnName("is_active");
        builder.HasIndex(table => new { table.EstablishmentId, table.Code }).IsUnique(); builder.HasIndex(table => table.QrCodeToken).IsUnique();
        builder.HasOne<OrderHub.Domain.Tenancy.Establishment>().WithMany().HasForeignKey(table => new { table.TenantId, table.EstablishmentId }).HasPrincipalKey(establishment => new { establishment.TenantId, establishment.Id }).OnDelete(DeleteBehavior.Restrict);
    }
}

internal sealed class BusinessHoursConfiguration : IEntityTypeConfiguration<BusinessHours>
{
    public void Configure(EntityTypeBuilder<BusinessHours> builder)
    {
        builder.ToTable("business_hours", DatabaseSchemas.Operations);
        builder.HasKey(hours => hours.Id); builder.Property(hours => hours.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(hours => hours.TenantId).HasColumnName("tenant_id"); builder.Property(hours => hours.EstablishmentId).HasColumnName("establishment_id");
        builder.Property(hours => hours.DayOfWeek).HasColumnName("day_of_week").HasConversion<short>(); builder.Property(hours => hours.OpensAt).HasColumnName("opens_at").HasColumnType("time without time zone"); builder.Property(hours => hours.ClosesAt).HasColumnName("closes_at").HasColumnType("time without time zone"); builder.Property(hours => hours.IsActive).HasColumnName("is_active");
        builder.ToTable(table => table.HasCheckConstraint("ck_business_hours_interval", "closes_at > opens_at"));
        builder.HasIndex(hours => new { hours.EstablishmentId, hours.DayOfWeek });
        builder.HasOne<OrderHub.Domain.Tenancy.Establishment>().WithMany().HasForeignKey(hours => new { hours.TenantId, hours.EstablishmentId }).HasPrincipalKey(establishment => new { establishment.TenantId, establishment.Id }).OnDelete(DeleteBehavior.Restrict);
    }
}
