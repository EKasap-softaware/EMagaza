using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Models;
using EMagaza.Infrastructure.Payments;
using Microsoft.Extensions.Logging.Abstractions;

namespace EMagaza.Tests.Fakes;

/// <summary>Gerçek simüle sağlayıcıyı sarar; hangi tutarların çekildiğini ve iade edildiğini kaydeder.</summary>
public class SpyPaymentGateway : IPaymentGateway
{
    private readonly SimulatedPaymentGateway _inner = new(new CardValidator(), NullLogger<SimulatedPaymentGateway>.Instance);

    public List<decimal> Charged { get; } = new();
    public List<string> Refunded { get; } = new();

    public async Task<PaymentResult> ChargeAsync(decimal amount, PaymentCard card, string reference)
    {
        var result = await _inner.ChargeAsync(amount, card, reference);
        if (result.Approved) Charged.Add(amount);
        return result;
    }

    public Task RefundAsync(string transactionId)
    {
        Refunded.Add(transactionId);
        return Task.CompletedTask;
    }
}
