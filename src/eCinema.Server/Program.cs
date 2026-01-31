using eCinema.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

await app.MapServicesAsync();

app.Run();