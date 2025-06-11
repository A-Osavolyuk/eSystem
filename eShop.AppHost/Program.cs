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

var emailService = builder.AddProject<Projects.eShop_EmailSender_Api>("email-sender-api", true)
    .WithHttpEndpoint(5104)
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var smsService = builder.AddProject<Projects.eShop_SmsSender_Api>("sms-service-api", true)
    .WithHttpEndpoint(5103)
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var telegramService = builder.AddProject<Projects.eShop_TelegramBot_Api>("telegram-service-api", true)
    .WithHttpEndpoint(5102)
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var messageBus = builder.AddProject<Projects.eShop_MessageBus>("message-bus", true)
    .WaitForReference(rabbitMq)
    .WithHttpEndpoint(5101)
    .WaitFor(emailService)
    .WaitFor(smsService)
    .WaitFor(telegramService);

var authApi = builder.AddProject<Projects.eShop_Auth_Api>("auth-api", true)
    .WithJwtConfig()
    .WithHttpEndpoint(5001)
    .WaitForReference(authDb)
    .WaitForReference(redisCache)
    .WaitForReference(rabbitMq)
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging");

var productApi = builder.AddProject<Projects.eShop_Product_Api>("product-api", true)
    .WithJwtConfig()
    .WithHttpEndpoint(5002)
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(productDb);

var reviewsApi = builder.AddProject<Projects.eShop_Comments_Api>("comment-api", true)
    .WithJwtConfig()
    .WithHttpEndpoint(5003)
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitForReference(commentsDb)
    .WaitForReference(redisCache)
    .WaitForReference(rabbitMq);

var cartApi = builder.AddProject<Projects.eShop_Cart_Api>("cart-api", true)
    .WithJwtConfig()
    .WithHttpEndpoint(5004)
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(cartDb);

var storageApi = builder.AddProject<Projects.eShop_Storage_Api>("storage-api", true)
    .WithJwtConfig()
    .WithHttpEndpoint(5005)
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(blobs);

var proxy = builder.AddProject<Projects.eShop_Proxy>("proxy", true)
    .WithHttpsEndpoint(5000)
    .WithJwtConfig()
    .WithReference(authApi).WaitFor(authApi)
    .WithReference(productApi).WaitFor(productApi)
    .WithReference(cartApi).WaitFor(cartApi)
    .WithReference(storageApi).WaitFor(storageApi)
    .WithReference(reviewsApi).WaitFor(reviewsApi);

builder.AddProject<Projects.eShop_BlazorWebUI>("blazor-webui", true)
    .WithJwtConfig()
    .WithHttpsEndpoint(5901)
    .WithReference(proxy).WaitFor(proxy).WithRelationship(proxy.Resource, "Proxy");

builder.AddNpmApp("angular-webui", "../eShop.AngularWebUI")
    .WaitFor(proxy).WithRelationship(proxy.Resource, "Proxy")
    .WithHttpsEndpoint(port: 5902, targetPort: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var app = builder.Build();

app.Run();