using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BackendWinder.Filters;

/// <summary>
/// Глобальный фильтр для обработки исключений
/// </summary>
public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionFilter(
        ILogger<GlobalExceptionFilter> logger,
        IHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        // Логируем ошибку
        _logger.LogError(
            context.Exception,
            "Произошла ошибка: {Message}",
            context.Exception.Message);

        object response;

        if (_environment.IsDevelopment())
        {
            response = new
            {
                message = context.Exception.Message,
                stackTrace = context.Exception.StackTrace
            };
        }
        else
        {
            response = new
            {
                message = "Внутренняя ошибка сервера"
            };
        }

        context.Result = new ObjectResult(response)
        {
            StatusCode = 500
        };

        context.ExceptionHandled = true;
    }
}