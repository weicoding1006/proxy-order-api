using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderSystem.Domain.Entities;

namespace OrderSystem.Api.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "Admin")]
public class RolesController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await roleManager.Roles
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request)
    {
        var result = await roleManager.CreateAsync(new IdentityRole(request.Name));
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        var role = await roleManager.FindByNameAsync(request.Name);
        return CreatedAtAction(nameof(GetRoles), new { id = role!.Id, name = role.Name });
    }

    [HttpPost("{roleId}/users")]
    public async Task<IActionResult> AssignUserToRole(string roleId, [FromBody] AssignUserRequest request)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is null) return NotFound("Role not found.");

        var user = await userManager.FindByIdAsync(request.UserId);
        if (user is null) return NotFound("User not found.");

        var result = await userManager.AddToRoleAsync(user, role.Name!);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok();
    }

    [HttpDelete("{roleId}/users/{userId}")]
    public async Task<IActionResult> RemoveUserFromRole(string roleId, string userId)
    {
        var role = await roleManager.FindByIdAsync(roleId);
        if (role is null) return NotFound("Role not found.");

        var user = await userManager.FindByIdAsync(userId);
        if (user is null) return NotFound("User not found.");

        var result = await userManager.RemoveFromRoleAsync(user, role.Name!);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return NoContent();
    }
}

public record CreateRoleRequest(string Name);
public record AssignUserRequest(string UserId);
