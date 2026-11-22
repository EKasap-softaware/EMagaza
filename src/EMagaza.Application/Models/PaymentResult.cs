namespace EMagaza.Application.Models;

public record PaymentResult(bool Approved, string? TransactionId, string? Brand, string? Last4, string? Error)
{
    public static PaymentResult Declined(string error) => new(false, null, null, null, error);
}
