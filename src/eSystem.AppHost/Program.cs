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

var postgres = builder.AddPostgres()
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

var eMessageServer = builder.AddProject<Projects.eSystem_MessageBus>("message-bus")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WaitFor(emailService)
    .WaitFor(smsService)
    .WaitFor(telegramService);

var eSecurityServer = builder.AddProject<Projects.eSecurity_Server>("e-security-server")
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WaitFor(eMessageServer).WithRelationship(eMessageServer.Resource, "Messaging");

var eStorageServer = builder.AddProject<Projects.eSystem_Storage_Api>("storage-api")
    .WaitFor(eSecurityServer).WithRelationship(eSecurityServer.Resource, "Authentication")
    .WaitFor(eMessageServer).WithRelationship(eMessageServer.Resource, "Messaging")
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(blobs).WaitFor(blobs);

var proxy = builder.AddProject<Projects.eSystem_Proxy>("proxy")
    .WithReference(eSecurityServer).WaitFor(eSecurityServer)
    .WithReference(eStorageServer).WaitFor(eStorageServer);

builder.AddProject<Projects.eSecurity_Client>("e-security-client")
    .WithReference(proxy).WaitFor(proxy);

var eCinemaServer = builder.AddProject<Projects.eCinema_Server>("e-cinema-server")
    .WaitFor(eSecurityServer)
    .WithReference(redisCache).WaitFor(redisCache)
    .WithReference(proxy).WaitFor(proxy);

builder.AddNpmApp("e-cinema-client", "../eCinema.Client")
    .WithReference(eCinemaServer).WaitFor(eCinemaServer)
    .WithHttpsEndpoint(port: 6511, targetPort: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var app = builder.Build();

app.Run();