using eSecurity.Client.BFF.Common.Cache;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisClient("redis");

builder.Services.AddControllers();
builder.Services.AddSingleton<ICache, RedisCache>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();