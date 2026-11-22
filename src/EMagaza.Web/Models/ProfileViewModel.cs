using EMagaza.Domain.Entities;

namespace EMagaza.Web.Models;

public class ProfileViewModel
{
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
    public IReadOnlyList<Address> Addresses { get; init; } = Array.Empty<Address>();
    public AddressForm NewAddress { get; init; } = new();
    public ChangePasswordForm Password { get; init; } = new();
}
