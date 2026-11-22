namespace EMagaza.Application.Interfaces.Infrastructure;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string storedHash);
}
