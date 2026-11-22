namespace EMagaza.Web.Helpers;

/// <summary>Bir ürün renginden türetilen üç ton: ana renk, gölge ve vurgu.</summary>
public record GarmentPalette(string Main, string Shade, string Light)
{
    public static GarmentPalette From(string hex) =>
        new(hex, ColorMixer.Mix(hex, "#000000", 0.22), ColorMixer.Mix(hex, "#ffffff", 0.35));
}
