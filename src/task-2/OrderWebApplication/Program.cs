using CustomConfigurationProvider.Provider.Entities;
using CustomConfigurationProvider.ProviderService.Entities;
using CustomConfigurationProvider.ProviderService.Interfaces;
using Microsoft.OpenApi.Models;
using Npgsql;
using OrderWebApplication;
using OrderWebApplication.BackgroundServices;
using OrderWebApplication.Middlewares;
using ProductService.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var customProvider = new CustomConfigProvider();
builder.Services.AddSingleton(customProvider);

builder.Configuration
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
    .Add(new CustomConfigurationProviderSource(customProvider))
    .Build();

builder.Services.AddApplicationOptions();
builder.Services.AddScoped(provider => provider.GetRequiredService<NpgsqlDataSourceBuilder>().Build());

builder.Services.AddSingleton<IProviderUpdatingService, CustomProviderUpdatingService>();
builder.Services.AddApplicationsMigrations();

builder.Services.AddHostedService<CustomProviderUpdatingBackgroundService>();
builder.Services.AddHostedService<MigrationBackgroundService>();

builder.Services.AddNpgSqlDataSourceBuilder();

builder.Services.AddScoped<ExceptionFormattingMiddleware>();
builder.Services.AddProductServices();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OrderAPI", Version = "v1" });
});

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionFormattingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.MapControllers();

app.Run();