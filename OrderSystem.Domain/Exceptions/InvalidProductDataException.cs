namespace OrderSystem.Domain.Exceptions;

public class InvalidProductDataException(string message) : Exception(message);
