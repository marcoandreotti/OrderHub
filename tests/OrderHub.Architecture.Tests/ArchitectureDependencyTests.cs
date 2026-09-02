using Microsoft.AspNetCore.Mvc;
using OrderHub.Application.Abstractions.Commands;
using OrderHub.Domain.Exceptions;
using OrderHub.Infrastructure.Persistence.Write;

namespace OrderHub.Architecture.Tests;

public sealed class ArchitectureDependencyTests
{
    [Fact]
    public void Domain_has_no_outward_layer_or_persistence_dependencies()
    {
        var forbidden = new[]
        {
            "OrderHub.Application",
            "OrderHub.Infrastructure",
            "OrderHub.Api",
            "Microsoft.EntityFrameworkCore",
            "Dapper",
            "Npgsql"
        };

        Assert.DoesNotContain(
            typeof(DomainException).Assembly.GetReferencedAssemblies(),
            reference => forbidden.Contains(reference.Name, StringComparer.Ordinal));
    }

    [Fact]
    public void Application_does_not_depend_on_api_or_infrastructure()
    {
        var referenced = typeof(ICommand).Assembly.GetReferencedAssemblies();
        Assert.DoesNotContain(referenced, reference => reference.Name is "OrderHub.Api" or "OrderHub.Infrastructure");
    }

    [Fact]
    public void Inner_layers_do_not_reference_the_migrations_project()
    {
        Assert.DoesNotContain(
            typeof(DomainException).Assembly.GetReferencedAssemblies(),
            reference => reference.Name == "OrderHub.Infrastructure.Migrations");
        Assert.DoesNotContain(
            typeof(ICommand).Assembly.GetReferencedAssemblies(),
            reference => reference.Name == "OrderHub.Infrastructure.Migrations");
    }

    [Fact]
    public void Controllers_do_not_receive_persistence_or_infrastructure_types()
    {
        var controllers = typeof(Program).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(ControllerBase).IsAssignableFrom(type));

        foreach (var controller in controllers)
        {
            var dependencies = controller.GetConstructors()
                .SelectMany(constructor => constructor.GetParameters())
                .Select(parameter => parameter.ParameterType)
                .Concat(controller.GetFields().Select(field => field.FieldType));

            Assert.DoesNotContain(dependencies, IsForbiddenControllerDependency);
        }
    }

    [Fact]
    public void Infrastructure_is_the_only_production_layer_referencing_ef_core()
    {
        Assert.Contains(
            typeof(OrderHubDbContext).Assembly.GetReferencedAssemblies(),
            reference => reference.Name == "Microsoft.EntityFrameworkCore");
        Assert.DoesNotContain(
            typeof(Program).Assembly.GetReferencedAssemblies(),
            reference => reference.Name == "Microsoft.EntityFrameworkCore");
    }

    private static bool IsForbiddenControllerDependency(Type type)
    {
        var namespaceName = type.Namespace ?? string.Empty;
        return namespaceName.StartsWith("OrderHub.Infrastructure", StringComparison.Ordinal)
            || namespaceName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
            || namespaceName.StartsWith("Dapper", StringComparison.Ordinal)
            || namespaceName.StartsWith("Npgsql", StringComparison.Ordinal);
    }
}
