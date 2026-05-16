using eSecurity.Client.BFF.Common.Cache;
using eSecurity.Client.BFF.Common.Proxy;
using eSecurity.Client.BFF.Data;
using eSystem.Core.Common.Gateway;
using eSystem.Core.Data;
using eSystem.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisClient("redis");
builder.AddSqlDb("e-security-bff-db");
builder.AddProxy();

builder.Services.AddGateway();
builder.Services.AddControllers();
builder.Services.AddSingleton<ICache, RedisCache>();

var app = builder.Build();

await app.ConfigureDatabaseAsync<AppDbContext>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapControllers();
app.MapDefaultEndpoints();
app.Run();