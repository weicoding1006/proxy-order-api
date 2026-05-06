using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs.Cart;
using OrderSystem.Application.DTOs.Order;
using OrderSystem.Application.Interfaces;
using OrderSystem.Domain.Exceptions;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(ICartService cartService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var cart = await cartService.GetOrCreateCartAsync(userId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var cart = await cartService.AddItemAsync(userId, dto.ProductId, dto.Quantity);
            return Ok(cart);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidProductDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{cartItemId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateItem(Guid cartItemId, [FromBody] UpdateCartItemRequest dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var cart = await cartService.UpdateItemQuantityAsync(userId, cartItemId, dto.Quantity);
            return Ok(cart);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await cartService.RemoveItemAsync(userId, cartItemId);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await cartService.ClearCartAsync(userId);
        return NoContent();
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderResponse>> Checkout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            var order = await cartService.CheckoutAsync(userId);
            return StatusCode(201, order);
        }
        catch (CartEmptyException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InsufficientStockException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidProductDataException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ProductNotFoundException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
