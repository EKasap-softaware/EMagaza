using EMagaza.Domain.Entities;

namespace EMagaza.Infrastructure.Persistence.Migrations;

/// <summary>`migrations` koleksiyonundaki kayıt: hangi migration ne zaman uygulandı.</summary>
public class AppliedMigration : Entity
{
    public int Version { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateTime AppliedAt { get; private set; } = DateTime.UtcNow;

    private AppliedMigration() { }

    public AppliedMigration(int version, string description)
    {
        Version = version;
        Description = description;
    }
}
