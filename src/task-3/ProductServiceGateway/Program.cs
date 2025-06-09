using Microsoft.Extensions.Options;
using ProductServiceGateway.Extensions;
using ProductServiceGateway.Middleware;
using ProductServiceGateway.Options;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpcClientOptions(builder.Configuration);
builder.Services.AddGrpcClient<GrpcProductService.Protos.MyOrderService.MyOrderServiceClient>((sp, o) =>
{
    IOptions<GrpcClientOptions> options = sp.GetRequiredService<IOptions<GrpcClientOptions>>();
    o.Address = new Uri(options.Value.Address);
});

builder.Services.AddGrpcClient<GrpcProductService.Protos.ProductService.ProductServiceClient>((sp, o) =>
{
    IOptions<GrpcClientOptions> options = sp.GetRequiredService<IOptions<GrpcClientOptions>>();
    o.Address = new Uri(options.Value.Address);
});

builder.Services.AddGrpcClient<Orders.ProcessingService.Contracts.OrderService.OrderServiceClient>((sp, o) =>
{
    IOptions<GrpcClientOptions> options = sp.GetRequiredService<IOptions<GrpcClientOptions>>();
    o.Address = new Uri(options.Value.ProcessingServiceAddress);
});

builder.Services.AddSingleton<ErrorTranslator>();

builder.Services.AddGrpcClientServices();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwagger();

WebApplication app = builder.Build();

app.UseMiddleware<ErrorTranslator>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

app.Run();