using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcProductService.Interceptors;

public class ErrorInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (RpcException ex)
        {
            Console.WriteLine($"Grpc error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}