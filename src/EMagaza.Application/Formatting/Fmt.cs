using System.Globalization;
namespace EMagaza.Application.Formatting;

/// <summary>Türkçe para/tarih biçimi. Sunucunun/konteynerin kültür ayarına BAĞLI KALMAZ (Docker'da çoğu zaman 'invariant' gelir).</summary>
public static class Fmt
{
    static readonly NumberFormatInfo Tr = new() { NumberGroupSeparator = ".", NumberDecimalSeparator = ",", NumberGroupSizes = new[] { 3 } };

    /// <summary>1299 → "1.299 ₺", 1299.9 → "1.299,90 ₺"</summary>
    public static string Money(decimal amount) =>
        (amount % 1 == 0 ? amount.ToString("N0", Tr) : amount.ToString("N2", Tr)) + " ₺";

    /// <summary>UTC → Türkiye saati (UTC+3, yaz/kış saati yok).</summary>
    public static string Date(DateTime utc) => utc.AddHours(3).ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
    public static string ShortDate(DateTime utc) => utc.AddHours(3).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
}
