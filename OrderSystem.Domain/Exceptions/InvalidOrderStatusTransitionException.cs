using OrderSystem.Domain.Enums;

namespace OrderSystem.Domain.Exceptions;

public class InvalidOrderStatusTransitionException(OrderStatus from, OrderStatus to)
    : Exception($"Invalid order status transition from '{from}' to '{to}'.");
