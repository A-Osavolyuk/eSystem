var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices();

var app = builder.Build();

await app.MapApiServices();

app.Run();