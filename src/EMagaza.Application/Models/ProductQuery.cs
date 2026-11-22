namespace EMagaza.Application.Models;

public record ProductQuery(
    string? Category = null,
    string? Type = null,
    string? Size = null,
    string? Text = null,
    ProductSort Sort = ProductSort.Featured,
    bool SaleOnly = false,
    bool NewOnly = false,
    int Page = 1,
    int PageSize = 12)
{
    /// <summary>Sayfalama olmadan tüm ürünler (panel ve ana sayfa vitrinleri için).</summary>
    public static ProductQuery Everything => new(PageSize: int.MaxValue);
}
