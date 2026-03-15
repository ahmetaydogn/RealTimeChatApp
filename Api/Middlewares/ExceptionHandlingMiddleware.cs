
using Business.Exceptions;

namespace Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate _requestDelegate, ILogger<ExceptionHandlingMiddleware> _logger)
        {
            requestDelegate = _requestDelegate;
            logger = _logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await requestDelegate.Invoke(context);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case KeyNotFoundException _:
                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status404NotFound, e.Message);
                        break;

                    // My own exception classes
                    case AuthenticationFailedException _:
                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status401Unauthorized, e.Message);
                        break;
                    case AuthorizationFailedException _:
                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status403Forbidden, e.Message);
                        break;
                    case ConflictException _:
                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status409Conflict, e.Message);
                        break;


                    case InvalidOperationException _:
                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status400BadRequest, e.Message);
                        break;

                    case ArgumentException _:
                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status400BadRequest, e.Message);
                        break;

                    case OperationCanceledException _:
                        logger.LogDebug("Request was cancelled.");
                        break;

                    default:
                        logger.LogError(e, "Unhandled exception occured.");

                        await MiddlewareHelper.ConfigureError(context, StatusCodes.Status500InternalServerError);
                        break;
                }
            }
        }
    }
}
