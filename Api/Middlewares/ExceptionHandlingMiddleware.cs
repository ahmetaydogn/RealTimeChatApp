
namespace Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(RequestDelegate _requestDelegate, ILogger<ExceptionHandlingMiddleware> _logger)
        {
            requestDelegate = _requestDelegate;
            logger = _logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception e)
            {
                switch (e)
                {
                    case e.
                    default:
                }
                throw;
            }
        }
    }
}
