namespace EMagaza.Domain.Entities;

public class Favorite : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public string ProductId { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Favorite() { }

    public Favorite(string userId, string productId)
    {
        UserId = userId;
        ProductId = productId;
    }
}
