namespace LoginApi.DTOs.Request;

public record UpdateUserRequestDTO(
        string? Name,
        string? Location
        );
