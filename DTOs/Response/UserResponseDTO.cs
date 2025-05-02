namespace LoginApi.DTOs.Response;

public record UserResponseDTO(
        string Id,
        string Name,
        string Email,
        string CreatedAt,
        string DeletedAt,
        string Location
        );
