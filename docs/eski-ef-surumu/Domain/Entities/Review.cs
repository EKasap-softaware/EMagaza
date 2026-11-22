namespace EMagaza.Domain.Entities;

/// <summary>Ürün yorumu. MongoDB'de saklanır (belge = JSON benzeri tek kayıt). Id: Mongo'nun ObjectId'si, string olarak.</summary>
public class Review
{
    public string Id { get; set; } = "";
    public int ProductId { get; set; }          // SQL'deki Product.Id'ye mantıksal bağ (veritabanları arası FK yoktur)
    public string Author { get; set; } = "";
    public int Rating { get; set; }             // 1-5
    public string Comment { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static Review Create(int productId, string author, int rating, string comment)
    {
        if (rating is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(rating), "Puan 1-5 arası olmalı.");
        if (string.IsNullOrWhiteSpace(author)) throw new ArgumentException("Ad zorunlu.", nameof(author));
        if (string.IsNullOrWhiteSpace(comment)) throw new ArgumentException("Yorum zorunlu.", nameof(comment));
        return new Review { ProductId = productId, Author = author.Trim(), Rating = rating, Comment = comment.Trim() };
    }
}
