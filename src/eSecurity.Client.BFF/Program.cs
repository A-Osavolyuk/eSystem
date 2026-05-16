using System.Text.Json.Serialization;
using eSecurity.Client.BFF.Common.Cache;
using eSecurity.Client.BFF.Common.Proxy;
using eSecurity.Client.BFF.Data;
using eSecurity.Client.BFF.Security.Authentication;
using eSecurity.Client.BFF.Security.Cors;
using eSystem.Core.Gateway;
using eSystem.Core.Server.Bff;
using eSystem.Core.Server.Data;
using eSystem.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisClient("redis");
builder.AddSqlDb("e-security-bff-db");
builder.AddProxy();
builder.AddCors();
builder.AddAuthentication();
builder.AddBff();

builder.Services.AddGateway();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDataProtection();
builder.Services.AddSingleton<ICache, RedisCache>();

builder.Services.AddControllers()
    .AddJsonOptions(cfg =>
    {
        cfg.JsonSerializerOptions.WriteIndented = true;
        cfg.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

await app.ConfigureDatabaseAsync<AppDbContext>();

app.UseRouting();
app.UseCors(CorsPolicies.SpaOnly);
app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapControllers();
app.MapDefaultEndpoints();
app.Run();