using EMagaza.Application.Models;
using EMagaza.Infrastructure.Payments;
using EMagaza.Tests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace EMagaza.Tests.Infrastructure;

public class PaymentGatewayTests
{
    private readonly SimulatedPaymentGateway _gateway = new(new CardValidator(), NullLogger<SimulatedPaymentGateway>.Instance);

    [Fact]
    public async Task ValidCard_IsApproved_AndOnlyLast4IsReturned()
    {
        var result = await _gateway.ChargeAsync(100, TestData.GoodCard, "u1");

        Assert.True(result.Approved);
        Assert.Equal("4242", result.Last4);
        Assert.Equal("Visa", result.Brand);
        Assert.StartsWith("SIM-", result.TransactionId);
    }

    [Fact]
    public async Task CardEndingWith0002_IsDeclined() =>
        Assert.False((await _gateway.ChargeAsync(100, TestData.DeclinedCard, "u1")).Approved);

    [Theory]
    [InlineData("1234 5678 9012 3456", "AD", "12/35", "123")]   // Luhn geçersiz
    [InlineData("4242 4242 4242 4242", "AD", "01/20", "123")]   // süresi geçmiş
    [InlineData("4242 4242 4242 4242", "AD", "13/35", "123")]   // ay geçersiz
    [InlineData("4242 4242 4242 4242", "AD", "12/35", "12")]    // CVC kısa
    [InlineData("4242 4242 4242 4242", "", "12/35", "123")]     // isim yok
    public async Task InvalidCardData_IsDeclined(string number, string holder, string expiry, string cvc) =>
        Assert.False((await _gateway.ChargeAsync(100, new PaymentCard(number, holder, expiry, cvc), "u1")).Approved);

    [Theory]
    [InlineData("5555 5555 5555 4444", "Mastercard")]
    [InlineData("3782 822463 10005", "Amex")]
    public async Task Brand_IsDetectedFromNumber(string number, string brand)
    {
        var result = await _gateway.ChargeAsync(10, new PaymentCard(number, "AD", "12/35", "1234"), "u1");

        Assert.Equal(brand, result.Brand);
    }
}
