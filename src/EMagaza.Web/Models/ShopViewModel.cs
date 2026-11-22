using EMagaza.Application.Models;
using EMagaza.Domain.Entities;

namespace EMagaza.Web.Models;

public class ShopViewModel
{
    public required ProductQuery Query { get; init; }
    public required PagedResult<Product> Result { get; init; }
    public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Types { get; init; } = Array.Empty<string>();

    public IReadOnlyList<Product> Products => Result.Items;
}
