using EMagaza.Application.Interfaces.Infrastructure;
using EMagaza.Application.Models;
using Microsoft.Extensions.Logging;

namespace EMagaza.Infrastructure.Payments;

/// <summary>
/// Gerçek para çekmeyen test sağlayıcısı. Geçerli her kart onaylanır; sonu 0002 ile biten kart reddedilir.
/// Kart numarası ve CVC saklanmaz/loglanmaz; sadece son 4 hane döner.
/// </summary>
public class SimulatedPaymentGateway(ICardValidator validator, ILogger<SimulatedPaymentGateway> logger) : IPaymentGateway
{
    private const string DeclinedCardSuffix = "0002";

    public async Task<PaymentResult> ChargeAsync(decimal amount, PaymentCard card, string reference)
    {
        await Task.Delay(600);   // ağ gecikmesi simülasyonu

        var error = validator.Validate(card);
        if (error is not null) return PaymentResult.Declined(error);

        var digits = CardNumber.DigitsOnly(card.Number);
        if (digits.EndsWith(DeclinedCardSuffix))
            return PaymentResult.Declined("Kartınız banka tarafından reddedildi (yetersiz bakiye).");

        var transactionId = $"SIM-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
        logger.LogInformation("Ödeme onaylandı: {TransactionId} {Amount} TL (kart ****{Last4})", transactionId, amount, CardNumber.Last4(digits));
        return new PaymentResult(true, transactionId, CardNumber.BrandOf(digits), CardNumber.Last4(digits), null);
    }

    public Task RefundAsync(string transactionId)
    {
        logger.LogWarning("Ödeme iade edildi: {TransactionId}", transactionId);
        return Task.CompletedTask;
    }
}
