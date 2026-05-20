using eSecurity.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

await app.MapServices();

app.Run();