using EMagaza.Domain.Exceptions;

namespace EMagaza.Domain.Entities;

/// <summary>Kullanıcının kayıtlı teslimat adresi (adres defteri).</summary>
public class Address : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Line { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Address() { }

    public Address(string userId, string title, string fullName, string phone, string city, string line)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new DomainException("Kullanıcı zorunlu.");
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Adres başlığı zorunlu (ör. Ev, İş).");
        if (string.IsNullOrWhiteSpace(fullName)) throw new DomainException("Ad soyad zorunlu.");
        if (phone is null || phone.Count(char.IsDigit) < 10) throw new DomainException("Geçerli bir telefon numarası girin.");
        if (string.IsNullOrWhiteSpace(city)) throw new DomainException("İl zorunlu.");
        if (string.IsNullOrWhiteSpace(line)) throw new DomainException("Adres zorunlu.");

        UserId = userId;
        Title = title.Trim();
        FullName = fullName.Trim();
        Phone = phone.Trim();
        City = city.Trim();
        Line = line.Trim();
    }
}
