var builder = DistributedApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(azurite =>
    {
        azurite.WithLifetime(ContainerLifetime.Persistent);
        azurite.WithDataVolume();
    });

var blobs = storage.AddBlobs("blobs");
var queue = storage.AddQueues("queue");
var table = storage.AddTables("table");

var redisCache = builder.AddRedis()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithRedisInsight(containerName: "redis-insights");

var sqlServer = builder.AddSqlServer()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var mongo = builder.AddMongoDb()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithMongoExpress(configuration);

var cartDb = mongo.AddDatabase("cart-db", "CartDB");
var authDb = sqlServer.AddDatabase("auth-db", "AuthDB");
var commentsDb = sqlServer.AddDatabase("comment-db", "CommentsDB");
var productDb = sqlServer.AddDatabase("product-db", "ProductDB");

var rabbitMq = builder.AddRabbitMq()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin()
    .WithDataVolume();

var gateway = builder.AddProject<Projects.eShop_Proxy>("proxy")
    .WithJwtConfig();

var emailService = builder.AddProject<Projects.eShop_EmailSender_Api>("email-sender-api")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var smsService = builder.AddProject<Projects.eShop_SmsSender_Api>("sms-service-api")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var telegramService = builder.AddProject<Projects.eShop_TelegramBot_Api>("telegram-service-api")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var messageBus = builder.AddProject<Projects.eShop_MessageBus>("message-bus")
    .WaitForReference(rabbitMq)
    .WaitFor(emailService)
    .WaitFor(smsService)
    .WaitFor(telegramService);

var authApi = builder.AddProject<Projects.eShop_Auth_Api>("auth-api")
    .WithJwtConfig()
    .WaitForReference(authDb)
    .WaitForReference(redisCache)
    .WaitForReference(rabbitMq)
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway");

var productApi = builder.AddProject<Projects.eShop_Product_Api>("product-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(productDb);

var reviewsApi = builder.AddProject<Projects.eShop_Comments_Api>("reviews-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(commentsDb)
    .WaitForReference(redisCache)
    .WaitForReference(rabbitMq);

var cartApi = builder.AddProject<Projects.eShop_Cart_Api>("cart-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(cartDb);

var filesStorageApi = builder.AddProject<Projects.eShop_Storage_Api>("storage-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(blobs);

builder.AddProject<Projects.eShop_BlazorWebUI>("blazor-webui")
    .WithJwtConfig()
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication");

builder.AddNpmApp("angular-webui", "../eShop.AngularWebUI")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WithHttpEndpoint(port: 40502, targetPort: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var app = builder.Build();

app.Run();