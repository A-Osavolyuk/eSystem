var builder = DistributedApplication.CreateBuilder(args);

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
    .WithDataVolume()
    .AddDatabase("auth-db", "AuthDB");

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

var securityServer = builder.AddProject<Projects.eSecurity_Server>("e-security-server")
    .WithReference(sqlServer).WaitFor(sqlServer)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging");

var storageApi = builder.AddProject<Projects.eSystem_Storage_Api>("storage-api")
    .WaitFor(securityServer).WithRelationship(securityServer.Resource, "Authentication")
    .WaitFor(messageBus).WithRelationship(messageBus.Resource, "Messaging")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(blobs).WaitFor(blobs);

var proxy = builder.AddProject<Projects.eSystem_Proxy>("proxy")
    .WithReference(securityServer).WaitFor(securityServer)
    .WithReference(storageApi).WaitFor(storageApi);

var securityClient = builder.AddProject<Projects.eSecurity_Client>("e-security-client")
    .WithReference(securityServer).WaitFor(securityServer)
    .WithReference(rabbitMq).WaitFor(proxy);

var app = builder.Build();

app.Run();