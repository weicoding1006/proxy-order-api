namespace OrderSystem.Domain.Exceptions;

public class ProductImageNotFoundException(Guid imageId)
    : Exception($"Product image with ID '{imageId}' was not found.");
