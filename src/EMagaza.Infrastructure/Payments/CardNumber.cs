namespace EMagaza.Infrastructure.Payments;

internal static class CardNumber
{
    public static string DigitsOnly(string? number) => new((number ?? string.Empty).Where(char.IsDigit).ToArray());

    public static string Last4(string digits) => digits[^4..];

    public static string BrandOf(string digits) => digits switch
    {
        _ when digits.StartsWith('4') => "Visa",
        _ when digits.StartsWith("34") || digits.StartsWith("37") => "Amex",
        _ when digits.StartsWith("9792") => "Troy",
        _ when digits.Length >= 2 && int.Parse(digits[..2]) is >= 51 and <= 55 => "Mastercard",
        _ => "Kart"
    };
}
