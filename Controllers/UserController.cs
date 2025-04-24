namespace LoginApi.controllers;

using System.Security.Claims;
using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using static BCrypt.Net.BCrypt;

[ApiController]
[Route("api/[controller]")]
public class UserController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    [HttpPost()]
    public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDto)
    {
        var hashedPassword = HashPassword(registerUserDto.Password);
        var user = new User(registerUserDto.Name, registerUserDto.Email, hashedPassword);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Created(string.Empty, new { id = user.Id });
    }

    [HttpGet()]
    [Authorize]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _context.Users.FindAsync(userId);

        return Ok(new
        {
            Data = new { User = user }
        });
    }
}
