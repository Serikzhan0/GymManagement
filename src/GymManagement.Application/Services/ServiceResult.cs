using Microsoft.AspNetCore.Http;

namespace GymManagementApi.Services;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
    public string? ErrorMessage { get; set; }
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
    public T? Data { get; set; }

    public static ServiceResult<T> Success(T data, int statusCode = StatusCodes.Status200OK) =>
        new() { IsSuccess = true, StatusCode = statusCode, Data = data };

    public static ServiceResult<T> Failure(string message, int statusCode = StatusCodes.Status400BadRequest, IDictionary<string, string[]>? errors = null) =>
        new() { IsSuccess = false, StatusCode = statusCode, ErrorMessage = message, ValidationErrors = errors };
}

public class ServiceResult : ServiceResult<object>
{
    public static ServiceResult Ok(string? message = null) =>
        new() { IsSuccess = true, StatusCode = StatusCodes.Status200OK, ErrorMessage = message };

    public static new ServiceResult Failure(string message, int statusCode = StatusCodes.Status400BadRequest, IDictionary<string, string[]>? errors = null) =>
        new() { IsSuccess = false, StatusCode = statusCode, ErrorMessage = message, ValidationErrors = errors };
}
