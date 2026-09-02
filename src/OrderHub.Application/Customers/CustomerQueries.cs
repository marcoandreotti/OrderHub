using OrderHub.Application.Abstractions.Customers;
using OrderHub.Application.Abstractions.Queries;

namespace OrderHub.Application.Customers;

public sealed record SearchCustomersQuery(
    Guid EstablishmentId,
    string? Search,
    int Page = 1,
    int PageSize = 20) : IQuery<CustomerSearchResult>;
