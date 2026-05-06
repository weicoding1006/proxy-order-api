namespace OrderSystem.Domain.Exceptions;

public class CartEmptyException() : Exception("Cart is empty. Add items before checking out.");
