namespace LoginApi.controllers;

using System.Security.Claims;
using LoginApi.Data;
using LoginApi.DTOs;
using LoginApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

using static BCrypt.Net.BCrypt;

[ApiController]
[Route("[controller]")]
public class UserController(AppDbContext context, IMapper mapper) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    [HttpPost()]
    public async Task<IActionResult> RegisterUser(RegisterUserDTO registerUserDto)
    {
        IsEmailInUse(registerUserDto.Email);
        var hashedPassword = HashPassword(registerUserDto.Password);
        var user = new User(registerUserDto.Name, registerUserDto.Email, hashedPassword);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Created(string.Empty, new { Message = "User created", Data = new { id = user.Id } });
    }

    [HttpGet("{uid}")]
    [Authorize]
    public async Task<IActionResult> GetUser(string uid)
    {
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();

        IsTargetUserEqualsToTokenUser(userIdFromToken, uid);
        var user = await FindUserByIdOrThrow(uid);

        return Ok(new
        {
            Data = new { User = user }
        });
    }

    [HttpGet()]
    [Authorize]
    public async Task<IActionResult> GetAllUser()
    {
        var users = await _context.Users.ToListAsync();

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

        var userToDelete = await FindUserByIdOrThrow(uid);

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

        var user = await FindUserByIdOrThrow(uid);
        _mapper.Map(updateUserDto, user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Data = new { User = user }
        });
    }

    private void IsEmailInUse(string email)
    {
        var emailExists = _context.Users.Any(u => u.Email == email);
        if (emailExists)
        {
            throw new BadHttpRequestException("E-mail already in use");
        }
    }

    private async Task<User> FindUserByIdOrThrow(string uid)
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
