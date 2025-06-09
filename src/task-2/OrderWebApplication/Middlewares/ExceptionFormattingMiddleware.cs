using System.Text;

namespace OrderWebApplication.Middlewares;

public class ExceptionFormattingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var builder = new StringBuilder();
            builder.Append($"Error occured in request: {context.Request.Body}\n");
            builder.Append($"Exception: {exception} \n");

            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(builder.ToString());
        }
    }
}