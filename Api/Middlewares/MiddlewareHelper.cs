namespace Api.Middlewares
{
    public static class MiddlewareHelper
    {
        public async static Task ConfigureError(HttpContext context, int statusCode, string message = "An unexpected error occurred")
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}
