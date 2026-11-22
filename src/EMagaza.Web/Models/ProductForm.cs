using System.ComponentModel.DataAnnotations;
using EMagaza.Application.Models;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Web.Models;

public class ProductForm
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Ürün adı zorunlu")] public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Marka zorunlu")] public string Brand { get; set; } = "LUNE";
    [Required(ErrorMessage = "Kategori zorunlu")] public string Category { get; set; } = "Kadın";
    [Required(ErrorMessage = "Ürün tipi zorunlu")] public string Type { get; set; } = "tshirt";
    [Required(ErrorMessage = "Renk adı zorunlu")] public string ColorName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Renk kodu zorunlu"), RegularExpression("^#[0-9a-fA-F]{6}$", ErrorMessage = "Renk #rrggbb biçiminde olmalı")]
    public string ColorHex { get; set; } = "#999999";
    [Range(1, 1_000_000, ErrorMessage = "Fiyat 1 ile 1.000.000 arasında olmalı")] public decimal Price { get; set; }
    [Range(0, 1_000_000, ErrorMessage = "Eski fiyat geçersiz")] public decimal OldPrice { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public bool IsNew { get; set; }

    /// <summary>"XS:5, S:4, M:10" biçiminde beden:stok listesi.</summary>
    [Required(ErrorMessage = "En az bir beden gerekli")] public string SizesText { get; set; } = string.Empty;

    public static ProductForm From(Product p) => new()
    {
        Id = p.Id, Name = p.Name, Brand = p.Brand, Category = p.Category, Type = p.Type, ColorName = p.ColorName,
        ColorHex = p.ColorHex, Price = p.Price, OldPrice = p.OldPrice, Description = p.Description, Material = p.Material,
        IsNew = p.IsNew, SizesText = string.Join(", ", p.Sizes.Select(s => $"{s.Size}:{s.Stock}"))
    };

    public ProductInput ToInput() => new(Name, Brand, Category, Type, ColorName, ColorHex, Price, OldPrice,
        Description ?? string.Empty, Material ?? string.Empty, IsNew, ParseSizes());

    private List<SizeInput> ParseSizes()
    {
        var sizes = new List<SizeInput>();
        foreach (var part in SizesText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pieces = part.Split(':', StringSplitOptions.TrimEntries);
            if (pieces.Length != 2 || pieces[0].Length == 0 || !int.TryParse(pieces[1], out var stock))
                throw new DomainException($"Beden biçimi hatalı: '{part}'. Doğru biçim: S:5, M:10");
            sizes.Add(new SizeInput(pieces[0], stock));
        }
        return sizes;
    }
}
