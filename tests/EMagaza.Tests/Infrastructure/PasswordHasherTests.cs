using EMagaza.Infrastructure.Security;

namespace EMagaza.Tests.Infrastructure;

public class PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void SamePassword_ProducesDifferentHashes_BecauseOfRandomSalt() =>
        Assert.NotEqual(_hasher.Hash("gizli123"), _hasher.Hash("gizli123"));

    [Fact]
    public void Verify_AcceptsCorrectPassword_RejectsOthers()
    {
        var hash = _hasher.Hash("gizli123");

        Assert.True(_hasher.Verify("gizli123", hash));
        Assert.False(_hasher.Verify("Gizli123", hash));
        Assert.False(_hasher.Verify("gizli123", "bozuk-format"));
    }
}
