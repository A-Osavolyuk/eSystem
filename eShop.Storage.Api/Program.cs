var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices();

var app = builder.Build();

app.MapAppServices();