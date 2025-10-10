public class ApiResponseDTO<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ApiResponseDTO(bool success, string? message = null, T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}