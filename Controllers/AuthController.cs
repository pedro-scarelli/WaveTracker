using LoginApi.Data;
using LoginApi.Services;
using LoginApi.DTOs.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BCrypt.Net.BCrypt;
namespace LoginApi.controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(AppDbContext context, JwtService jwt) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly JwtService _jwt = jwt;

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDTO loginRequestDto)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginRequestDto.Email);
        if (user == null || !Verify(loginRequestDto.Password, user.HashedPassword))
            return Unauthorized();

        var token = _jwt.GenerateToken(user.Id.ToString(), user.Email);

        return Ok(new { Data = new { Token = token }, Message = "Login efetuado com sucesso" });
    }
}
