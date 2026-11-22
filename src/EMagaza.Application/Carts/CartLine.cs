namespace EMagaza.Application.Carts;

public record CartLine(
    string ProductId,
    string Size,
    string Name,
    string Brand,
    string Type,
    string ColorName,
    string ColorHex,
    decimal UnitPrice,
    int Quantity)
{
    public decimal LineTotal => UnitPrice * Quantity;
}
