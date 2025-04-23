
using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;
using static BCrypt.Net.BCrypt;
namespace LoginApi.controllers;

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
        return Created();
    }
}
