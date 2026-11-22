using EMagaza.Domain.Entities;
namespace EMagaza.Infrastructure.Persistence;

public static class DataSeeder
{
    public static void Seed(StoreDbContext db)
    {
        if (db.Products.Any()) return;
        db.Products.AddRange(
            P("Kablosuz Kulaklık", "Aktif gürültü engelleme, 30 saat pil.", 2499, 25, "Elektronik", "🎧", "#6366f1"),
            P("Akıllı Saat", "Nabız, uyku ve adım takibi.", 3299, 15, "Elektronik", "⌚", "#0ea5e9"),
            P("Mekanik Klavye", "Mavi switch, RGB aydınlatma.", 1799, 30, "Elektronik", "⌨️", "#8b5cf6"),
            P("Bluetooth Hoparlör", "Suya dayanıklı, 12 saat müzik.", 1299, 40, "Elektronik", "🔊", "#ec4899"),
            P("Sırt Çantası", "Su geçirmez, laptop bölmeli.", 899, 50, "Moda", "🎒", "#f59e0b"),
            P("Güneş Gözlüğü", "UV400 korumalı polarize cam.", 649, 60, "Moda", "🕶️", "#14b8a6"),
            P("Spor Ayakkabı", "Hafif taban, nefes alan kumaş.", 1999, 20, "Moda", "👟", "#ef4444"),
            P("Termos Bardak", "12 saat sıcak, 24 saat soğuk.", 349, 80, "Ev", "🥤", "#10b981"),
            P("Masa Lambası", "Dokunmatik, ayarlanabilir ışık.", 549, 35, "Ev", "💡", "#eab308"),
            P("Kahve Makinesi", "Filtre kahve, 1.2 L hazne.", 1599, 12, "Ev", "☕", "#a16207"),
            P("Yoga Matı", "Kaymaz yüzey, 6 mm kalınlık.", 399, 45, "Spor", "🧘", "#22c55e"),
            P("Dambıl Seti", "2 x 5 kg, kaplamalı.", 799, 18, "Spor", "🏋️", "#64748b"));
        db.SaveChanges();
    }

    static Product P(string n, string d, decimal price, int stock, string cat, string emoji, string color) =>
        new() { Name = n, Description = d, Price = price, Stock = stock, Category = cat, Emoji = emoji, Color = color };
}
