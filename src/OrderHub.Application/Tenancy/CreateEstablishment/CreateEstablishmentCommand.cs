using OrderHub.Application.Abstractions.Commands;

namespace OrderHub.Application.Tenancy.CreateEstablishment;

public sealed record CreateEstablishmentCommand(string TradeName, string Slug) : ICommand<Guid>;
