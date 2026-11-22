using EMagaza.Domain.Entities;

namespace EMagaza.Web.Models;

public class HomeViewModel
{
    public IReadOnlyList<Product> New { get; init; } = Array.Empty<Product>();
    public IReadOnlyList<Product> Sale { get; init; } = Array.Empty<Product>();
    public IReadOnlyList<Product> Best { get; init; } = Array.Empty<Product>();
}
