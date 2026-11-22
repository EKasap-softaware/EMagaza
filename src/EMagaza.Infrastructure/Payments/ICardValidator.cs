using EMagaza.Application.Models;

namespace EMagaza.Infrastructure.Payments;

public interface ICardValidator
{
    /// <summary>Kart geçerliyse null, değilse kullanıcıya gösterilecek hata mesajı.</summary>
    string? Validate(PaymentCard card);
}
