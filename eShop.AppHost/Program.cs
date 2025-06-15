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

var emailService = builder.AddProject<Projects.eShop_EmailSender_Api>("email-sender-api")
    .WithHttpEndpoint(5104)
    .WithReference(rabbitMq)
    .WithReference(redisCache);

var smsService = builder.AddProject<Projects.eShop_SmsSender_Api>("sms-service-api")
    .WithHttpEndpoint(5103)
    .WithReference(rabbitMq)
    .WithReference(redisCache);

var telegramService = builder.AddProject<Projects.eShop_TelegramBot_Api>("telegram-service-api")
    .WithReference(rabbitMq)
    .WithReference(redisCache);

var messageBus = builder.AddProject<Projects.eShop_MessageBus>("message-bus")
    .WithReference(rabbitMq)
    .WaitFor(emailService)
    .WaitFor(smsService)
    .WaitFor(telegramService);

var authApi = builder.AddProject<Projects.eShop_Auth_Api>("auth-api")
    .WithJwtConfig()
    .WithReference(authDb)
    .WithReference(redisCache)
    .WithReference(rabbitMq)
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging");

var productApi = builder.AddProject<Projects.eShop_Product_Api>("product-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq)
    .WithReference(redisCache)
    .WithReference(productDb);

var reviewsApi = builder.AddProject<Projects.eShop_Comments_Api>("comment-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(commentsDb)
    .WithReference(redisCache)
    .WithReference(rabbitMq);

var cartApi = builder.AddProject<Projects.eShop_Cart_Api>("cart-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq)
    .WithReference(redisCache)
    .WithReference(cartDb);

var storageApi = builder.AddProject<Projects.eShop_Storage_Api>("storage-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq)
    .WithReference(redisCache)
    .WithReference(blobs);

var proxy = builder.AddProject<Projects.eShop_Proxy>("proxy")
    .WithJwtConfig()
    .WithReference(authApi).WaitFor(authApi)
    .WithReference(productApi).WaitFor(productApi)
    .WithReference(cartApi).WaitFor(cartApi)
    .WithReference(storageApi).WaitFor(storageApi)
    .WithReference(reviewsApi).WaitFor(reviewsApi);

builder.AddProject<Projects.eShop_BlazorWebUI>("blazor-webui")
    .WithJwtConfig()
    .WithReference(proxy).WaitFor(proxy).WithRelationship(proxy.Resource, "Proxy");

builder.AddNpmApp("angular-webui", "../eShop.AngularWebUI")
    .WaitFor(proxy).WithRelationship(proxy.Resource, "Proxy")
    .WithHttpsEndpoint(port: 5902, targetPort: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var app = builder.Build();

app.Run();