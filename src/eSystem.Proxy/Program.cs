using eSystem.Proxy.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AppApiServices();

var app = builder.Build();

app.MapApiServices();

app.Run();