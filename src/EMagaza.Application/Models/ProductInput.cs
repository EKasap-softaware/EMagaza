namespace EMagaza.Application.Models;

/// <summary>Yönetici panelinden gelen ürün bilgisi.</summary>
public record ProductInput(
    string Name,
    string Brand,
    string Category,
    string Type,
    string ColorName,
    string ColorHex,
    decimal Price,
    decimal OldPrice,
    string Description,
    string Material,
    bool IsNew,
    IReadOnlyList<SizeInput> Sizes);
