namespace LoginApi.controllers;

using System.Security.Claims;
using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using static BCrypt.Net.BCrypt;

[ApiController]
[Route("[controller]")]
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

    [HttpGet("{uid}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string uid)
    {
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        IsTargetUserEqualsToTokenUser(userIdFromToken, uid);
        var user = await FindUserById(uid);

        return Ok(new
        {
            Message = "User created",
            Data = new { User = user }
        });
    }

    [HttpGet()]
    [Authorize]
    public async Task<IActionResult> GetAllUser()
    {
        var users = await _context.Users.FindAsync();

        return Ok(new
        {
            Data = new { Users = users }
        });
    }

    [HttpDelete("{uid}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(string uid)
    {
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        IsTargetUserEqualsToTokenUser(userIdFromToken, uid);

        var userToDelete = await FindUserById(uid);

        userToDelete.DeletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "User deleted"
        });
    }

    [HttpPatch("{uid}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser(string uid, UpdateUserDTO updateUserDto)
    {
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        IsTargetUserEqualsToTokenUser(userIdFromToken, uid);
        var user = await FindUserById(uid);
        if (updateUserDto.Name != null)
        {
            user.Name = updateUserDto.Name;
        }

        return Ok(new
        {
            Data = new { User = user }
        });
    }

    private async Task<User> FindUserById(string uid)
    {
        var foundUser = await _context.Users.FindAsync(new Guid(uid));

        if (foundUser != null)
        {
            return foundUser;
        }

        throw new KeyNotFoundException("User not found");
    }

    private static void IsTargetUserEqualsToTokenUser(string requestUserId, string targetUserId)
    {
        if (requestUserId != targetUserId)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
