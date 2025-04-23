namespace LoginApi.DTOs;

public record RegisterUserDto(
        string Name,
        string Email,
        string Password
        );
