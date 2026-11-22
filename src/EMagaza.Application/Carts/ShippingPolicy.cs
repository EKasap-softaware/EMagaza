namespace EMagaza.Application.Carts;

public static class ShippingPolicy
{
    public const decimal FreeThreshold = 1500m;
    public const decimal Fee = 59.90m;

    public static decimal FeeFor(decimal subtotal) => subtotal <= 0 || subtotal >= FreeThreshold ? 0 : Fee;
}
