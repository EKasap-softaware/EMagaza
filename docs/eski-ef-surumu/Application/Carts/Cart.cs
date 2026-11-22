namespace EMagaza.Application.Carts;

public record CartLine(int ProductId, string Name, decimal UnitPrice, int Quantity, string Emoji)
{
    public decimal LineTotal => UnitPrice * Quantity;
}

/// <summary>Saf (veritabanı bilmeyen) sepet sınıfı. Unit test için ideal.</summary>
public class Cart
{
    public List<CartLine> Lines { get; set; } = new();
    public decimal Total => Lines.Sum(l => l.LineTotal);
    public int ItemCount => Lines.Sum(l => l.Quantity);

    public void Add(int productId, string name, decimal price, string emoji, int quantity = 1)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        var i = Lines.FindIndex(l => l.ProductId == productId);
        if (i >= 0) Lines[i] = Lines[i] with { Quantity = Lines[i].Quantity + quantity };
        else Lines.Add(new CartLine(productId, name, price, quantity, emoji));
    }

    public void Remove(int productId) => Lines.RemoveAll(l => l.ProductId == productId);
    public void Clear() => Lines.Clear();
}
