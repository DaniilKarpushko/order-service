using Confluent.Kafka;
using CustomConfigurationProvider.ProviderService.Entities;
using CustomConfigurationProvider.ProviderService.Interfaces;
using GrpcProductService.ExtensionServices;
using GrpcProductService.Interceptors;
using GrpcProductService.Services.Entities;
using Kafka.Consumer;
using Kafka.Consumer.Entities;
using Kafka.Extensions;
using Kafka.Producer;
using Kafka.Producer.Entities;
using Kafka.Serializer;
using Orders.Kafka.Contracts;
using OrderWebApplication;
using OrderWebApplication.BackgroundServices;
using OrderWebApplication.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
    .Build();

builder.Services.AddApplicationOptions();

builder.Services.AddSingleton<IProviderUpdatingService, CustomProviderUpdatingService>();
builder.Services.AddApplicationsMigrations();
builder.Services.AddKafkaOptions(builder.Configuration);

builder.Services.AddTransient<IDeserializer<OrderProcessingKey>, KafkaMessageSerializer<OrderProcessingKey>>();
builder.Services.AddTransient<IDeserializer<OrderProcessingValue>, KafkaMessageSerializer<OrderProcessingValue>>();

builder.Services.AddTransient<ISerializer<OrderCreationKey>, KafkaMessageSerializer<OrderCreationKey>>();
builder.Services.AddTransient<ISerializer<OrderCreationValue>, KafkaMessageSerializer<OrderCreationValue>>();

builder.Services.AddSingleton<IKafkaProducer<OrderCreationKey, OrderCreationValue>, KafkaProducer<OrderCreationKey, OrderCreationValue>>();
builder.Services.AddSingleton<IKafkaConsumer<OrderProcessingKey, OrderProcessingValue>, KafkaConsumer<OrderProcessingKey, OrderProcessingValue>>();
builder.Services.AddProductServicesNew(builder.Configuration);
builder.Services.AddSingleton<IMessageHandler<OrderProcessingKey, OrderProcessingValue>, OrderMessageHandler>();

builder.Services.AddHostedService<MigrationBackgroundService>();
builder.Services.AddHostedService<KafkaMessageReaderService<OrderProcessingKey, OrderProcessingValue>>();

builder.Services.AddNpgSqlDataSourceBuilder();

builder.Services.AddScoped<ExceptionFormattingMiddleware>();
builder.Services.AddGrpc(grpc => grpc.Interceptors.Add<ErrorInterceptor>());
builder.Services.AddGrpcReflection();

WebApplication app = builder.Build();

app.MapGrpcService<MyOrderServiceGrpc>();
app.MapGrpcService<ProductServiceGrpc>();

app.Run();