using eShop.MessageBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServices();

var app = builder.Build();

app.MapServices();

app.Run();