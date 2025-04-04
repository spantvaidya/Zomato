using System.Net;

namespace Zomato.Web.Middlewares
{
    /// <summary>
    /// Global Exception Middlewares
    /// </summary>
    public class GlobalExceptionHandler(RequestDelegate requestDelegate, ILogger<GlobalExceptionHandler> logger)
    {
        private readonly RequestDelegate _requestDelegate = requestDelegate;
        private readonly ILogger<GlobalExceptionHandler> _logger = logger;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex} ");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode status;
            var stackTrace = exception.StackTrace;
            string message;

            status = GetHttpStatusCode(exception);
            message = GetMessage(exception);

            var exceptionResult = System.Text.Json.JsonSerializer.Serialize(new
            {
                status,
                error = message,
                stackTrace
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            await context.Response.WriteAsync(exceptionResult);
        }
        private HttpStatusCode GetHttpStatusCode(Exception exception)
        {
            if (exception is ArgumentException)
                return HttpStatusCode.BadRequest;
            else if (exception.Message.Contains(ErrorConstants.ValidationErrorMessage))
                return HttpStatusCode.BadRequest;
            else if (exception.Message.Contains(ErrorConstants.NotFoundMessage))
                return HttpStatusCode.NotFound;
            else if (exception is NotImplementedException)
                return HttpStatusCode.NotImplemented;
            else if (exception is UnauthorizedAccessException || exception is KeyNotFoundException)
                return HttpStatusCode.Unauthorized;
            else if (exception.Message.Contains(ErrorConstants.TransientFailureMessage))
                return HttpStatusCode.BadRequest;
            else
                return HttpStatusCode.InternalServerError;
        }
        private string GetMessage(Exception exception)
        {
            if (exception is ArgumentException)
                return ErrorConstants.BadRequestMessage;
            else if (exception.Message.Contains(ErrorConstants.ValidationErrorMessage))
                return ErrorConstants.ValidationErrorMessage;
            else if (exception.Message.Contains(ErrorConstants.NotFoundMessage))
                return ErrorConstants.NotFoundMessage;
            else if (exception is NotImplementedException)
                return ErrorConstants.NotImplementedExceptionMessage;
            else if (exception is UnauthorizedAccessException || exception is KeyNotFoundException)
                return ErrorConstants.UnauthorizedMessage;
            else if (exception.Message.Contains(ErrorConstants.TransientFailureMessage))
                return ErrorConstants.SQLServerError;
            else
                return "An unexpected error occurred";
        }

    }
}
