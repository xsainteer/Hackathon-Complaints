using Infrastructure.Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Authorize(Roles = "SuperAdmin")]
[ApiController]
[Route("api/[controller]")]
public class SuperAdminController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public SuperAdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        this._userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("MakeAdmin/{userId}")]
    public async Task<IActionResult> MakeAdminById(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound($"User with Id {userId} not found.");

        var roleExists = await _roleManager.RoleExistsAsync("Admin");
        if (!roleExists)
            await _roleManager.CreateAsync(new IdentityRole("Admin"));

        var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
        if (isInRole)
            return BadRequest("User is already an Admin.");

        var result = await _userManager.AddToRoleAsync(user, "Admin");
        if (result.Succeeded)
            return Ok("User was successfully made Admin.");

        return BadRequest(result.Errors);
    }

    [HttpPost("MakeAdminByName/{username}")]
    public async Task<IActionResult> MakeAdminByName(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound($"User with username {username} not found.");

        var roleExists = await _roleManager.RoleExistsAsync("Admin");
        if (!roleExists)
            await _roleManager.CreateAsync(new IdentityRole("Admin"));

        var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
        if (isInRole)
            return BadRequest("User is already an Admin.");

        var result = await _userManager.AddToRoleAsync(user, "Admin");
        if (result.Succeeded)
            return Ok("User was successfully made Admin.");

        return BadRequest(result.Errors);
    }
}