using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

public class Review : Entity
{
    public string ProductId { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public string Comment { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Review() { }

    public Review(string productId, string userId, string author, int rating, string comment)
    {
        if (string.IsNullOrWhiteSpace(productId)) throw new DomainException("Ürün zorunlu.");
        if (rating is < 1 or > 5) throw new DomainException("Puan 1-5 arası olmalı.");
        if (string.IsNullOrWhiteSpace(author)) throw new DomainException("Ad zorunlu.");
        if (string.IsNullOrWhiteSpace(comment)) throw new DomainException("Yorum zorunlu.");

        ProductId = productId;
        UserId = userId;
        Author = author.Trim();
        Rating = rating;
        Comment = comment.Trim();
    }
}
