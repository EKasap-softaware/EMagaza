using System.Text.RegularExpressions;
using EMagaza.Application.Models;

namespace EMagaza.Infrastructure.Payments;

public partial class CardValidator : ICardValidator
{
    public string? Validate(PaymentCard card)
    {
        var digits = CardNumber.DigitsOnly(card.Number);

        if (digits.Length is < 13 or > 19 || !PassesLuhn(digits)) return "Kart numarası geçersiz.";
        if (string.IsNullOrWhiteSpace(card.Holder)) return "Kart üzerindeki isim zorunlu.";
        if (!IsExpiryValid(card.Expiry)) return "Son kullanma tarihi geçersiz veya geçmiş (AA/YY).";
        if (!CvcPattern().IsMatch(card.Cvc ?? string.Empty)) return "CVC 3 veya 4 haneli olmalı.";
        return null;
    }

    private static bool PassesLuhn(string digits)
    {
        var sum = 0;
        var doubleIt = false;
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            var digit = digits[i] - '0';
            if (doubleIt && (digit *= 2) > 9) digit -= 9;
            sum += digit;
            doubleIt = !doubleIt;
        }
        return sum % 10 == 0;
    }

    private static bool IsExpiryValid(string? expiry)
    {
        var match = ExpiryPattern().Match(expiry ?? string.Empty);
        if (!match.Success) return false;

        var month = int.Parse(match.Groups[1].Value);
        var year = 2000 + int.Parse(match.Groups[2].Value);
        var now = DateTime.UtcNow;
        return month is >= 1 and <= 12 && (year > now.Year || (year == now.Year && month >= now.Month));
    }

    [GeneratedRegex(@"^(\d{2})\s*/\s*(\d{2})$")]
    private static partial Regex ExpiryPattern();

    [GeneratedRegex(@"^\d{3,4}$")]
    private static partial Regex CvcPattern();
}
