using LoginApi.Data;
using LoginApi.Services;
using LoginApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;
namespace LoginApi.controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext context, JwtService jwt) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly JwtService _jwt = jwt;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDTO loginDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null || !Verify(loginDto.Password, user.HashedPassword))
            return Unauthorized();

        var token = _jwt.GenerateToken(user.Id.ToString(), user.Email);
        return Ok(new { Data = new { Token = token }, Message = "Login efetuado com sucesso" });
    }
}
