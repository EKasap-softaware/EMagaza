namespace EMagaza.Application.Options;

public class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>Bu e-postayla kayıt olan kişi Admin olur.</summary>
    public string AdminEmail { get; set; } = "admin@emagaza.local";
}
