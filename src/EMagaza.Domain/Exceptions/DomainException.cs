namespace EMagaza.Domain.Exceptions;

/// <summary>İş kuralı ihlali. Kullanıcıya gösterilebilecek bir mesaj taşır.</summary>
public class DomainException(string message) : Exception(message);
