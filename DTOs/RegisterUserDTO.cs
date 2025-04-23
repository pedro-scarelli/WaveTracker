namespace LoginApi.DTOs;

public record RegisterUserDTO(
        string Name,
        string Email,
        string Password
        );
