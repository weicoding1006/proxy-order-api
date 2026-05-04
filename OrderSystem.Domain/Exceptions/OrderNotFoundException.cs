namespace OrderSystem.Domain.Exceptions;

public class OrderNotFoundException(Guid id)
    : Exception($"Order '{id}' not found.");
