namespace EMagaza.Application.Models;

/// <summary>Doğrulanmış bir kupon ve bu sepet için hesaplanan indirim tutarı.</summary>
public record CouponApplication(string Code, decimal Discount);
