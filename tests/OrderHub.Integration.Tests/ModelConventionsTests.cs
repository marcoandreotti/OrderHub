using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using OrderHub.Domain.Tenancy;
using OrderHub.Infrastructure.Persistence.Write;
using OrderHub.Domain.Catalog;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OrderHub.Domain.Customers;

namespace OrderHub.Integration.Tests;

public sealed class ModelConventionsTests
{
    [Fact]
    public void Tenancy_metadata_uses_expected_schema_columns_and_delete_behavior()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>()
            .UseNpgsql("Host=localhost;Database=metadata;Username=metadata;Password=metadata")
            .Options;
        using var context = new OrderHubDbContext(options);
        var model = context.Model;

        var establishment = model.FindEntityType(typeof(Establishment))!;
        var table = StoreObjectIdentifier.Table("establishment", "tenancy");

        Assert.Equal("tenancy", establishment.GetSchema());
        Assert.Equal("tenant_id", establishment.FindProperty(nameof(Establishment.TenantId))!.GetColumnName(table));
        Assert.Equal("timestamp with time zone", establishment.FindProperty(nameof(Establishment.CreatedAt))!.GetColumnType());
        Assert.Contains(establishment.GetIndexes(), index => index.IsUnique && index.GetDatabaseName() == "ux_establishment_slug");
        Assert.Equal(DeleteBehavior.Restrict, Assert.Single(establishment.GetForeignKeys()).DeleteBehavior);
    }

    [Fact]
    public void Catalog_metadata_has_scoped_keys_constraints_and_restrictive_links()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql("Host=localhost;Database=metadata;Username=metadata;Password=metadata").Options;
        using var context = new OrderHubDbContext(options); var model = context.Model;
        var product = model.FindEntityType(typeof(Product))!; var category = model.FindEntityType(typeof(Category))!;
        Assert.Equal("catalog", product.GetSchema());
        Assert.Contains(product.GetIndexes(), index => index.IsUnique && index.Properties.Select(p => p.Name).SequenceEqual([nameof(Product.TenantId), nameof(Product.EstablishmentId), nameof(Product.Code)]));
        Assert.Contains(product.GetForeignKeys(), key => key.DeleteBehavior == DeleteBehavior.Restrict && key.PrincipalEntityType.ClrType == typeof(Category));
        var designCategory = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Category))!;
        Assert.Contains(designCategory.GetCheckConstraints(), check => check.Name == "ck_category_order");
    }

    [Fact]
    public void Customer_metadata_has_scoped_contact_and_single_primary_constraints()
    {
        var options = new DbContextOptionsBuilder<OrderHubDbContext>().UseNpgsql("Host=localhost;Database=metadata;Username=metadata;Password=metadata").Options;
        using var context = new OrderHubDbContext(options);
        var customer = context.Model.FindEntityType(typeof(Customer))!;
        var address = context.Model.FindEntityType(typeof(CustomerAddress))!;

        Assert.Equal("customers", customer.GetSchema());
        Assert.Contains(customer.GetIndexes(), index => index.IsUnique && index.Properties.Select(property => property.Name).SequenceEqual([nameof(Customer.TenantId), nameof(Customer.EstablishmentId), nameof(Customer.NormalizedPhone)]));
        Assert.True(customer.FindProperty(nameof(Customer.Version))!.IsConcurrencyToken);
        Assert.Contains(address.GetIndexes(), index => index.IsUnique && index.GetFilter() == "is_primary");
        Assert.Equal(DeleteBehavior.Cascade, Assert.Single(address.GetForeignKeys()).DeleteBehavior);
    }
}
