using Microsoft.AspNetCore.Html;

namespace EMagaza.Web.Helpers;

/// <summary>Ürün görselleri: fotoğraf yerine ürün rengiyle boyanan vektör çizimler.</summary>
public static class Garments
{
    private const string FallbackShape = "<rect x='50' y='40' width='100' height='170' rx='12' fill='{0}'/>";
    private const string GroundShadow = "<ellipse cx='100' cy='231' rx='54' ry='5' fill='#000' fill-opacity='.08'/>";

    private static readonly IReadOnlyDictionary<string, string> TypeLabels = new Dictionary<string, string>
    {
        ["dress"] = "Elbise", ["tshirt"] = "Tişört", ["shirt"] = "Gömlek", ["sweater"] = "Kazak", ["hoodie"] = "Sweatshirt",
        ["pants"] = "Pantolon", ["jeans"] = "Kot", ["jacket"] = "Ceket", ["coat"] = "Mont & Kaban", ["skirt"] = "Etek",
        ["bag"] = "Çanta", ["scarf"] = "Atkı & Şal"
    };

    public static string Label(string type) => TypeLabels.GetValueOrDefault(type, type);

    /// <summary>Kart arka planı: ürün renginin açık tonu.</summary>
    public static string Tint(string hex) => ColorMixer.Mix(hex, "#f6f1e9", 0.86);

    public static IHtmlContent Svg(string type, string hex)
    {
        var shape = GarmentShapes.All.TryGetValue(type, out var draw)
            ? draw(GarmentPalette.From(hex))
            : string.Format(FallbackShape, hex);

        return new HtmlString($"<svg class='garment' viewBox='0 0 200 240' xmlns='http://www.w3.org/2000/svg' role='img' aria-label='{Label(type)}'>{GroundShadow}{shape}</svg>");
    }
}
