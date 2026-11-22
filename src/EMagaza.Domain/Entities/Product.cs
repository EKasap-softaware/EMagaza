using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

public class Product : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Brand { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Material { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public string ColorName { get; private set; } = string.Empty;
    public string ColorHex { get; private set; } = "#999999";
    public decimal Price { get; private set; }
    public decimal OldPrice { get; private set; }
    public bool IsNew { get; private set; }
    public int Popularity { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public int RatingSum { get; private set; }
    public int ReviewCount { get; private set; }
    public List<SizeStock> Sizes { get; private set; } = new();

    public double RatingAverage => ReviewCount == 0 ? 0 : (double)RatingSum / ReviewCount;
    public int TotalStock => Sizes.Sum(s => s.Stock);
    public bool OnSale => OldPrice > Price;
    public int DiscountPercent => OnSale ? (int)Math.Round((OldPrice - Price) / OldPrice * 100) : 0;

    private Product() { }

    public Product(string name, string brand, string category, string type, string colorName, string colorHex, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Ürün adı zorunlu.");
        if (string.IsNullOrWhiteSpace(category)) throw new DomainException("Kategori zorunlu.");
        if (price <= 0) throw new DomainException("Fiyat 0'dan büyük olmalı.");
        Name = name.Trim();
        Brand = brand;
        Category = category;
        Type = type;
        ColorName = colorName;
        ColorHex = colorHex;
        Price = price;
    }

    public void UpdateDetails(string name, string brand, string category, string type, string colorName, string colorHex,
        decimal price, decimal oldPrice, string description, string material, bool isNew)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Ürün adı zorunlu.");
        if (string.IsNullOrWhiteSpace(category)) throw new DomainException("Kategori zorunlu.");
        if (price <= 0) throw new DomainException("Fiyat 0'dan büyük olmalı.");
        if (oldPrice != 0 && oldPrice <= price) throw new DomainException("Eski fiyat güncel fiyattan yüksek olmalı (indirim yoksa 0 bırak).");

        Name = name.Trim();
        Brand = brand;
        Category = category;
        Type = type;
        ColorName = colorName;
        ColorHex = colorHex;
        Price = price;
        OldPrice = oldPrice;
        Description = description;
        Material = material;
        IsNew = isNew;
    }

    public void ReplaceSizes(IEnumerable<(string Size, int Stock)> sizes)
    {
        var list = sizes.Select(s => new SizeStock(s.Size, s.Stock)).ToList();
        if (list.Count == 0) throw new DomainException("En az bir beden gerekli.");
        if (list.GroupBy(s => s.Size).Any(g => g.Count() > 1)) throw new DomainException("Aynı beden iki kez girilemez.");
        Sizes = list;
    }

    public Product AddSize(string size, int stock)
    {
        if (Sizes.Any(s => s.Size == size)) throw new DomainException($"{size} bedeni zaten tanımlı.");
        Sizes.Add(new SizeStock(size, stock));
        return this;
    }

    public Product WithDiscount(decimal oldPrice)
    {
        if (oldPrice <= Price) throw new DomainException("Eski fiyat güncel fiyattan yüksek olmalı.");
        OldPrice = oldPrice;
        return this;
    }

    public int StockOf(string size) => FindSize(size)?.Stock ?? 0;

    public void DecreaseStock(string size, int quantity) =>
        (FindSize(size) ?? throw new DomainException($"'{Name}' için '{size}' bedeni yok.")).Decrease(quantity);

    private SizeStock? FindSize(string size) => Sizes.FirstOrDefault(s => s.Size == size);
}
