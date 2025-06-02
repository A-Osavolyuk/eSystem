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

var gateway = builder.AddProject<Projects.eShop_Proxy>("proxy");

var messageBus = builder.AddProject<Projects.eShop_MessageBus>("message-bus")
    .WaitForReference(rabbitMq);

var emailService = builder.AddProject<Projects.eShop_EmailSender_Api>("email-sender-api")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var smsService = builder.AddProject<Projects.eShop_SmsSender_Api>("sms-service-api")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var telegramService = builder.AddProject<Projects.eShop_TelegramBot_Api>("telegram-service-api")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache);

var authApi = builder.AddProject<Projects.eShop_Auth_Api>("auth-api")
    .WaitForReference(authDb)
    .WaitForReference(redisCache)
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway");

var productApi = builder.AddProject<Projects.eShop_Product_Api>("product-api")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(productDb);

var reviewsApi = builder.AddProject<Projects.eShop_Comments_Api>("reviews-api")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(commentsDb)
    .WaitForReference(redisCache)
    .WaitForReference(rabbitMq);

var cartApi = builder.AddProject<Projects.eShop_Cart_Api>("cart-api")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(cartDb);

var filesStorageApi = builder.AddProject<Projects.eShop_Files_Api>("file-store-api")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitForReference(rabbitMq)
    .WaitForReference(redisCache)
    .WaitForReference(blobs);

var blazorClient = builder.AddProject<Projects.eShop_BlazorWebUI>("blazor-webui")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway");

var angularClient = builder.AddNpmApp("angular-webui", "../eShop.AngularWebUI")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(gateway).WithRelationship(gateway.Resource, "Gateway")
    .WithHttpEndpoint(port: 40502, targetPort: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var app = builder.Build();

app.Run();