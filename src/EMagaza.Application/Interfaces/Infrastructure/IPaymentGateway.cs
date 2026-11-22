using EMagaza.Application.Models;

namespace EMagaza.Application.Interfaces.Infrastructure;

public interface IPaymentGateway
{
    Task<PaymentResult> ChargeAsync(decimal amount, PaymentCard card, string reference);
    Task RefundAsync(string transactionId);
}
