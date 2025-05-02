namespace LoginApi.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using LoginApi.DTOs.Response;

public class ApiResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var originalResult = context.Result;
        var statusCode = GetStatusCode(context, originalResult);
        var (data, message) = ExtractDataAndMessage(originalResult);

        context.Result = new ObjectResult(new ApiResponseDTO<object>
        {
            Timestamp = DateTime.UtcNow,
            StatusCode = statusCode,
            Data = data,
            Message = message
        })
        {
            StatusCode = statusCode
        };

        await next();
    }

    private static int GetStatusCode(ResultExecutingContext context, IActionResult result)
    {
        return result switch
        {
            ObjectResult obj => obj.StatusCode ?? context.HttpContext.Response.StatusCode,
            StatusCodeResult code => code.StatusCode,
            _ => context.HttpContext.Response.StatusCode
        };
    }

    private static (object? Data, string? Message) ExtractDataAndMessage(IActionResult result)
    {
        if (result is not ObjectResult { Value: not null } objResult)
            return (null, null);

        var value = objResult.Value;
        var data = value.GetType().GetProperty("Data")?.GetValue(value) ?? null;
        var message = value switch
        {
            ProblemDetails problem => problem.Title,
            _ => value.GetType().GetProperty("Message")?.GetValue(value)?.ToString()
        };

        return (data, message);
    }
}
