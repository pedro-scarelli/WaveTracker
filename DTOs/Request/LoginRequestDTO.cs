namespace LoginApi.DTOs.Request;

public record LoginRequestDTO(
        string Email,
        string Password
        );
