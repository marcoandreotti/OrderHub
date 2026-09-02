using OrderHub.Domain.Exceptions;
using OrderHub.Domain.Operations;

namespace OrderHub.Domain.Tests.Operations;

public sealed class OperationsTests
{
    [Fact]
    public void Table_token_is_opaque_unique_and_revocable()
    {
        var table = ServiceTable.Create(Guid.NewGuid(), Guid.NewGuid(), " mesa-1 ");
        var token = table.QrCodeToken;
        Assert.Equal("MESA-1", table.Code); Assert.Equal(64, token.Length);
        table.RevokeToken(); Assert.NotEqual(token, table.QrCodeToken);
    }

    [Fact]
    public void Hours_require_same_day_interval_and_support_multiple_ranges()
    {
        var tenant = Guid.NewGuid(); var establishment = Guid.NewGuid();
        var lunch = BusinessHours.Create(tenant, establishment, DayOfWeek.Monday, new TimeOnly(11, 0), new TimeOnly(14, 0));
        var dinner = BusinessHours.Create(tenant, establishment, DayOfWeek.Monday, new TimeOnly(18, 0), new TimeOnly(23, 0));
        Assert.True(lunch.Contains(DayOfWeek.Monday, new TimeOnly(12, 0))); Assert.True(dinner.Contains(DayOfWeek.Monday, new TimeOnly(20, 0)));
        Assert.False(lunch.Contains(DayOfWeek.Monday, new TimeOnly(16, 0)));
        Assert.Throws<DomainException>(() => BusinessHours.Create(tenant, establishment, DayOfWeek.Monday, new TimeOnly(23, 0), new TimeOnly(2, 0)));
    }
}
