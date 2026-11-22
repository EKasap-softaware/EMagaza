namespace EMagaza.Web.Helpers;

public static class ColorMixer
{
    /// <summary>İki #rrggbb rengi karıştırır: ratio 0 → a, 1 → b.</summary>
    public static string Mix(string a, string b, double ratio)
    {
        var (r1, g1, b1) = Parse(a);
        var (r2, g2, b2) = Parse(b);
        return $"#{Blend(r1, r2, ratio):x2}{Blend(g1, g2, ratio):x2}{Blend(b1, b2, ratio):x2}";
    }

    private static int Blend(int from, int to, double ratio) => (int)Math.Round(from + (to - from) * ratio);

    private static (int R, int G, int B) Parse(string hex)
    {
        hex = hex.TrimStart('#');
        return (Convert.ToInt32(hex[..2], 16), Convert.ToInt32(hex[2..4], 16), Convert.ToInt32(hex[4..6], 16));
    }
}
