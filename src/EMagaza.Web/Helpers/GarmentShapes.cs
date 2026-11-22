namespace EMagaza.Web.Helpers;

/// <summary>Her ürün tipinin vektör çizimi. Yeni tip eklemek için bir metot yazıp <see cref="All"/> listesine eklemek yeterli.</summary>
public static class GarmentShapes
{
    public static readonly IReadOnlyDictionary<string, Func<GarmentPalette, string>> All = new Dictionary<string, Func<GarmentPalette, string>>
    {
        ["tshirt"] = Tshirt,
        ["hoodie"] = Hoodie,
        ["sweater"] = Sweater,
        ["shirt"] = Shirt,
        ["dress"] = Dress,
        ["skirt"] = Skirt,
        ["pants"] = Pants,
        ["jeans"] = Jeans,
        ["jacket"] = Jacket,
        ["coat"] = Coat,
        ["bag"] = Bag,
        ["scarf"] = Scarf,
    };

    private static string Tshirt(GarmentPalette p) => $@"<path d='M62 24 L86 14 Q100 30 114 14 L138 24 L186 64 L162 98 L142 82 L142 218 L58 218 L58 82 L38 98 L14 64 Z' fill='{p.Main}'/>
<path d='M58 82 L58 218 L78 218 L78 96 Z' fill='{p.Shade}' fill-opacity='.30'/>
<path d='M86 14 Q100 30 114 14 Q100 46 86 14 Z' fill='{p.Shade}'/>
<path d='M62 24 L58 82 M138 24 L142 82' stroke='{p.Shade}' stroke-opacity='.5' stroke-width='1.4' fill='none'/>";

    private static string Hoodie(GarmentPalette p) => $@"<path d='M60 34 L84 22 L116 22 L140 34 L188 152 L162 162 L140 102 L140 218 L60 218 L60 102 L38 162 L12 152 Z' fill='{p.Main}'/>
<path d='M72 30 Q100 -4 128 30 Q112 62 100 62 Q88 62 72 30 Z' fill='{p.Shade}'/>
<path d='M83 31 Q100 8 117 31 Q108 50 100 50 Q92 50 83 31 Z' fill='#000' fill-opacity='.38'/>
<path d='M76 158 L124 158 L131 192 L69 192 Z' fill='{p.Shade}' fill-opacity='.45'/>
<path d='M93 56 L92 88 M107 56 L108 88' stroke='{p.Light}' stroke-width='2.5' stroke-linecap='round'/>
<rect x='60' y='206' width='80' height='12' fill='{p.Shade}' fill-opacity='.5'/>
<path d='M14 150 L38 160 L34 170 L10 160 Z' fill='{p.Shade}' fill-opacity='.5'/><path d='M186 150 L162 160 L166 170 L190 160 Z' fill='{p.Shade}' fill-opacity='.5'/>";

    private static string Sweater(GarmentPalette p) => $@"<path d='M60 30 L84 20 L116 20 L140 30 L188 152 L162 162 L140 102 L140 218 L60 218 L60 102 L38 162 L12 152 Z' fill='{p.Main}'/>
<path d='M80 20 Q100 44 120 20 L124 30 Q100 58 76 30 Z' fill='{p.Shade}'/>
<rect x='60' y='204' width='80' height='14' fill='{p.Shade}' fill-opacity='.45'/>
<path d='M14 150 L38 160 L34 172 L10 162 Z' fill='{p.Shade}' fill-opacity='.5'/><path d='M186 150 L162 160 L166 172 L190 162 Z' fill='{p.Shade}' fill-opacity='.5'/>
<path d='M70 60 L70 200 M82 60 L82 200 M118 60 L118 200 M130 60 L130 200' stroke='{p.Shade}' stroke-opacity='.18' stroke-width='2'/>";

    private static string Shirt(GarmentPalette p) => $@"<path d='M60 30 L84 20 L116 20 L140 30 L188 152 L162 162 L140 102 L140 218 L60 218 L60 102 L38 162 L12 152 Z' fill='{p.Main}'/>
<path d='M84 18 L100 46 L86 56 L70 32 Z' fill='#fff' fill-opacity='.92'/><path d='M116 18 L100 46 L114 56 L130 32 Z' fill='#fff' fill-opacity='.92'/>
<path d='M100 46 L100 218' stroke='{p.Shade}' stroke-opacity='.55' stroke-width='1.6'/>
<circle cx='100' cy='72' r='2.2' fill='{p.Shade}'/><circle cx='100' cy='102' r='2.2' fill='{p.Shade}'/><circle cx='100' cy='132' r='2.2' fill='{p.Shade}'/><circle cx='100' cy='162' r='2.2' fill='{p.Shade}'/><circle cx='100' cy='192' r='2.2' fill='{p.Shade}'/>
<rect x='108' y='84' width='24' height='26' rx='2' fill='none' stroke='{p.Shade}' stroke-opacity='.5' stroke-width='1.3'/>
<path d='M14 150 L38 160 L34 172 L10 162 Z' fill='{p.Shade}' fill-opacity='.5'/><path d='M186 150 L162 160 L166 172 L190 162 Z' fill='{p.Shade}' fill-opacity='.5'/>";

    private static string Dress(GarmentPalette p) => $@"<path d='M76 18 L90 16 Q100 34 110 16 L124 18 L130 72 L120 96 L160 224 Q100 240 40 224 L80 96 L70 72 Z' fill='{p.Main}'/>
<path d='M90 16 Q100 34 110 16 Q100 50 90 16 Z' fill='{p.Shade}'/>
<path d='M80 96 L120 96 L123 108 L77 108 Z' fill='{p.Shade}' fill-opacity='.6'/>
<path d='M90 110 L70 228 M100 110 L100 232 M110 110 L130 228' stroke='{p.Shade}' stroke-opacity='.28' stroke-width='2'/>";

    private static string Skirt(GarmentPalette p) => $@"<path d='M68 40 L132 40 L166 216 Q100 236 34 216 Z' fill='{p.Main}'/>
<rect x='68' y='32' width='64' height='13' rx='3' fill='{p.Shade}'/>
<path d='M80 46 L58 222 M90 46 L80 226 M100 46 L100 228 M110 46 L120 226 M120 46 L142 222' stroke='{p.Shade}' stroke-opacity='.45' stroke-width='1.8'/>";

    private static string Pants(GarmentPalette p) => $@"<path d='M58 22 L142 22 L154 224 L108 224 L100 98 L92 224 L46 224 Z' fill='{p.Main}'/>
<rect x='58' y='22' width='84' height='13' fill='{p.Shade}'/>
<path d='M100 35 L100 98' stroke='{p.Shade}' stroke-opacity='.6' stroke-width='1.6'/>
<path d='M62 38 Q76 56 78 74 M138 38 Q124 56 122 74' stroke='{p.Shade}' stroke-opacity='.55' stroke-width='1.6' fill='none'/>
<path d='M66 120 L66 224 M134 120 L134 224' stroke='{p.Shade}' stroke-opacity='.18' stroke-width='3'/>";

    private static string Jeans(GarmentPalette p) => $@"<path d='M58 22 L142 22 L154 224 L108 224 L100 98 L92 224 L46 224 Z' fill='{p.Main}'/>
<rect x='58' y='22' width='84' height='13' fill='{p.Shade}'/>
<path d='M100 35 L100 98' stroke='{p.Light}' stroke-width='1.6' stroke-dasharray='3 2.5'/>
<path d='M62 38 Q76 58 80 78 M138 38 Q124 58 120 78' stroke='{p.Light}' stroke-width='1.6' stroke-dasharray='3 2.5' fill='none'/>
<path d='M52 150 L66 150 M134 150 L148 150' stroke='{p.Shade}' stroke-opacity='.35' stroke-width='6'/>
<circle cx='100' cy='30' r='2.6' fill='{p.Light}'/>
<path d='M64 218 L96 218 M104 218 L136 218' stroke='{p.Light}' stroke-width='1.4' stroke-dasharray='3 2.5'/>";

    private static string Jacket(GarmentPalette p) => $@"<path d='M60 28 L84 18 L116 18 L140 28 L188 150 L162 160 L140 100 L140 214 L60 214 L60 100 L38 160 L12 150 Z' fill='{p.Main}'/>
<path d='M100 40 L100 214' stroke='{p.Shade}' stroke-width='2'/>
<path d='M86 16 L100 74 L68 58 Z' fill='{p.Light}' fill-opacity='.9'/><path d='M114 16 L100 74 L132 58 Z' fill='{p.Light}' fill-opacity='.9'/>
<circle cx='94' cy='110' r='2.6' fill='{p.Shade}'/><circle cx='94' cy='140' r='2.6' fill='{p.Shade}'/><circle cx='94' cy='170' r='2.6' fill='{p.Shade}'/>
<rect x='66' y='150' width='26' height='6' rx='2' fill='{p.Shade}' fill-opacity='.6'/><rect x='108' y='150' width='26' height='6' rx='2' fill='{p.Shade}' fill-opacity='.6'/>
<path d='M14 148 L38 158 L34 170 L10 160 Z' fill='{p.Shade}' fill-opacity='.45'/><path d='M186 148 L162 158 L166 170 L190 160 Z' fill='{p.Shade}' fill-opacity='.45'/>";

    private static string Coat(GarmentPalette p) => $@"<path d='M58 24 L84 14 L116 14 L142 24 L190 172 L164 182 L142 114 L148 234 L52 234 L58 114 L36 182 L10 172 Z' fill='{p.Main}'/>
<path d='M100 40 L100 234' stroke='{p.Shade}' stroke-width='2'/>
<path d='M84 12 L100 84 L62 62 Z' fill='{p.Light}' fill-opacity='.85'/><path d='M116 12 L100 84 L138 62 Z' fill='{p.Light}' fill-opacity='.85'/>
<rect x='56' y='122' width='88' height='10' fill='{p.Shade}' fill-opacity='.7'/><rect x='94' y='119' width='12' height='16' rx='2' fill='{p.Light}'/>
<circle cx='92' cy='156' r='2.8' fill='{p.Shade}'/><circle cx='108' cy='156' r='2.8' fill='{p.Shade}'/><circle cx='92' cy='186' r='2.8' fill='{p.Shade}'/><circle cx='108' cy='186' r='2.8' fill='{p.Shade}'/><circle cx='92' cy='216' r='2.8' fill='{p.Shade}'/><circle cx='108' cy='216' r='2.8' fill='{p.Shade}'/>
<path d='M12 170 L36 180 L32 192 L8 182 Z' fill='{p.Shade}' fill-opacity='.45'/><path d='M188 170 L164 180 L168 192 L192 182 Z' fill='{p.Shade}' fill-opacity='.45'/>";

    private static string Bag(GarmentPalette p) => $@"<path d='M68 98 Q68 28 100 28 Q132 28 132 98' fill='none' stroke='{p.Shade}' stroke-width='7' stroke-linecap='round'/>
<path d='M40 92 L160 92 L174 218 L26 218 Z' fill='{p.Main}'/>
<path d='M40 92 L160 92 L156 128 Q100 150 44 128 Z' fill='{p.Shade}' fill-opacity='.45'/>
<circle cx='100' cy='138' r='6.5' fill='{p.Light}'/>
<path d='M44 200 L156 200' stroke='{p.Shade}' stroke-opacity='.3' stroke-width='1.5' stroke-dasharray='4 3'/>";

    private static string Scarf(GarmentPalette p) => $@"<path d='M82 74 L114 74 L120 216 L74 216 Z' fill='{p.Main}'/>
<path d='M106 78 L138 72 L150 208 L110 216 Z' fill='{p.Shade}'/>
<ellipse cx='100' cy='58' rx='58' ry='28' fill='{p.Main}'/>
<ellipse cx='100' cy='62' rx='36' ry='14' fill='{p.Shade}' fill-opacity='.5'/>
<path d='M80 218 L80 230 M88 218 L88 230 M96 218 L96 230 M112 218 L112 230 M120 218 L120 230 M132 212 L134 226 M142 210 L144 224' stroke='{p.Shade}' stroke-width='2' stroke-linecap='round'/>";
}
