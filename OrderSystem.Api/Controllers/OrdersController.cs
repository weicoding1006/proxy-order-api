using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs.Order;
using OrderSystem.Application.Services;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(OrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var order = await orderService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidProductDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderResponse>>> GetAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var orders = await orderService.FindByUserIdAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var order = await orderService.FindOneAsync(id, userId);
        if (order is null)
            return NotFound();
        return Ok(order);
    }
}
