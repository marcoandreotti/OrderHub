using OrderHub.Domain.Exceptions;

namespace OrderHub.Domain.SharedKernel;

public readonly record struct Quantity
{
    public decimal Value { get; }

    public Quantity(decimal value)
    {
        if (value <= 0m)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        var rounded = decimal.Round(value, 3, MidpointRounding.AwayFromZero);
        if (rounded <= 0m)
        {
            throw new DomainException("Quantity is below the supported precision.");
        }

        Value = rounded;
    }
}
