namespace LoginApi.DTOs.Response;

public class ApiResponseDTO<T>
{
        public DateTime Timestamp { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
}
