namespace LoginApi.DTOs.Request;

public record RegisterUserRequestDTO(
        string Name,
        string Email,
        string Password,
        string Location
        );
