using eShop.BlazorWebUI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppServices();

var app = builder.Build();

app.MapAppServices();

app.Run();