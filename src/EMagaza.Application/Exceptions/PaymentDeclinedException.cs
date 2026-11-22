using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Exceptions;

public class PaymentDeclinedException(string message) : DomainException(message);
