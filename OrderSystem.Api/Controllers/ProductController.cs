using Microsoft.AspNetCore.Mvc;
using OrderSystem.Application.DTOs;
using OrderSystem.Application.Services;
using OrderSystem.Domain.Exceptions;
using Microsoft.AspNetCore.Http;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ProductResponseDto>>> GetAll()
    {
        var products = await productService.FindAllAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
    {
        try
        {
            var product = await productService.FindOneAsync(id);
            return Ok(product);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto dto)
    {
        var created = await productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        try
        {
            var updated = await productService.UpdateAsync(id, dto);
            return Ok(updated);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await productService.RemoveAsync(id);
            return NoContent();
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/images")]
    public async Task<ActionResult<ProductImageDto>> UploadImage(Guid id, IFormFile file)
    {
        try
        {
            var image = await productService.UploadImageAsync(id, file);
            return CreatedAtAction(nameof(GetById), new { id }, image);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(Guid id, Guid imageId)
    {
        try
        {
            await productService.RemoveImageAsync(id, imageId);
            return NoContent();
        }
        catch (ProductImageNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/images/{imageId:guid}/set-cover")]
    public async Task<ActionResult<ProductImageDto>> SetCoverImage(Guid id, Guid imageId)
    {
        try
        {
            var image = await productService.SetCoverImageAsync(id, imageId);
            return Ok(image);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ProductImageNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
