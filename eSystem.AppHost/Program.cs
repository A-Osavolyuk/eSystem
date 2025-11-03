var builder = DistributedApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(azurite =>
    {
        azurite.WithLifetime(ContainerLifetime.Persistent);
        azurite.WithDataVolume();
    });

var blobs = storage.AddBlobs("blobs");

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

var emailService = builder.AddProject<Projects.eSystem_EmailSender_Api>("email-sender")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache);

var smsService = builder.AddProject<Projects.eSystem_SmsSender_Api>("sms-sender")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache);

var telegramService = builder.AddProject<Projects.eSystem_Telegram_Bot>("telegram-bot")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache);

var messageBus = builder.AddProject<Projects.eSystem_MessageBus>("message-bus")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WaitFor(emailService)
    .WaitFor(smsService)
    .WaitFor(telegramService);

var authApi = builder.AddProject<Projects.eSecurity>("e-security")
    .WithJwtConfig()
    .WithReference(authDb).WaitFor(authDb)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging");

var productApi = builder.AddProject<Projects.eSystem_Product_Api>("product-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(rabbitMq)
    .WithReference(productDb).WaitFor(productDb);

var commentApi = builder.AddProject<Projects.eSystem_Comments_Api>("comment-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(commentsDb).WaitFor(commentsDb)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(rabbitMq).WaitFor(rabbitMq);

var cartApi = builder.AddProject<Projects.eSystem_Cart_Api>("cart-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(cartDb).WaitFor(cartDb);

var storageApi = builder.AddProject<Projects.eSystem_Storage_Api>("storage-api")
    .WithJwtConfig()
    .WaitFor(authApi).WithRelationship(authApi.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(blobs).WaitFor(blobs);

var proxy = builder.AddProject<Projects.eSystem_Proxy>("proxy")
    .WithJwtConfig()
    .WithReference(authApi).WaitFor(authApi)
    .WithReference(productApi).WaitFor(productApi)
    .WithReference(cartApi).WaitFor(cartApi)
    .WithReference(storageApi).WaitFor(storageApi)
    .WithReference(commentApi).WaitFor(commentApi);

builder.AddProject<Projects.eAccount>("e-account")
    .WithJwtConfig()
    .WithReference(proxy).WaitFor(proxy).WithRelationship(proxy.Resource, "Proxy");

var app = builder.Build();

app.Run();