namespace OrderSystem.Domain.Exceptions;

public class InsufficientStockException(string productName, int available, int requested)
    : Exception($"Insufficient stock for '{productName}': available {available}, requested {requested}.");
