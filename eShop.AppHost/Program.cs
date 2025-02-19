using eShop.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var redisCache = builder.AddRedis()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithRedisInsight(containerName: "redis-insights");

var sqlServer = builder.AddSqlServer()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume();

var mongo = builder.AddMongoDB()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume()
    .WithMongoExpress(configuration);

var cartDb = mongo.AddDatabase("cart-db", "CartDB");
var authDb = sqlServer.AddDatabase("auth-db", "AuthDB");
var reviewsDb = sqlServer.AddDatabase("reviews-db", "ReviewsDB");
var productDb = sqlServer.AddDatabase("product-db", "ProductDB");

var rabbitMq = builder.AddRabbitMQ()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin()
    .WithDataVolume();

var emailService = builder.AddProject<Projects.eShop_EmailSender_Api>("email-sender-api")
    .WithReference(rabbitMq);

var smsService = builder.AddProject<Projects.eShop_SmsSender_Api>("sms-service-api")
    .WaitForReference(rabbitMq);

var telegramService = builder.AddProject<Projects.eShop_TelegramBot_Api>("telegram-service-api")
    .WaitForReference(rabbitMq);

var authApi = builder.AddProject<Projects.eShop_Auth_Api>("auth-api")
    .WaitForReference(sqlServer)
    .WaitForReference(emailService)
    .WaitForReference(smsService)
    .WaitForReference(telegramService)
    .WaitForReference(redisCache);

var productApi = builder.AddProject<Projects.eShop_Product_Api>("product-api")
    .WaitForReference(authApi);

var reviewsApi = builder.AddProject<Projects.eShop_Comments_Api>("reviews-api")
    .WaitForReference(authApi);

var cartApi = builder.AddProject<Projects.eShop_Cart_Api>("cart-api")
    .WaitForReference(authApi);

var filesStorageApi = builder.AddProject<Projects.eShop_Files_Api>("file-store-api")
    .WaitForReference(authApi);

var gateway = builder.AddProject<Projects.eShop_Proxy>("proxy");

var blazorClient = builder.AddProject<Projects.eShop_BlazorWebUI>("blazor-webui")
    .WaitForReference(gateway)
    .WaitForReference(authApi);

var angularClient = builder.AddNpmApp("angular-webui",
        "../eShop.AngularWebUI")
    .WaitForReference(gateway)
    .WaitForReference(authApi)
    .WithHttpEndpoint(port: 40502, targetPort: 4200, env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

var app = builder.Build();

app.Run();