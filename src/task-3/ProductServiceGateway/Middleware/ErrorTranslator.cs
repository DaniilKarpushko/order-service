using Grpc.Core;

namespace ProductServiceGateway.Middleware;

public class ErrorTranslator : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (RpcException ex)
        {
            context.Response.StatusCode = ex.StatusCode switch
            {
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.Internal => StatusCodes.Status500InternalServerError,
                StatusCode.OK => StatusCodes.Status200OK,
                StatusCode.Cancelled => StatusCodes.Status499ClientClosedRequest,
                StatusCode.Unknown => StatusCodes.Status502BadGateway,
                StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
                StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
                StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                StatusCode.ResourceExhausted => StatusCodes.Status429TooManyRequests,
                StatusCode.FailedPrecondition => StatusCodes.Status412PreconditionFailed,
                StatusCode.Aborted => StatusCodes.Status409Conflict,
                StatusCode.OutOfRange => StatusCodes.Status400BadRequest,
                StatusCode.Unimplemented => StatusCodes.Status501NotImplemented,
                StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
                StatusCode.DataLoss => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError,
            };
            await context.Response.WriteAsync(ex.Status.Detail);
        }
    }
}