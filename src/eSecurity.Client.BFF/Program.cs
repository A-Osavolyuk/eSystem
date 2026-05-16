using eSecurity.Client.BFF.Common.Cache;
using eSecurity.Client.BFF.Data;
using eSystem.Core.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisClient("redis");
builder.AddSqlDb("e-security-bff-db");

builder.Services.AddControllers();
builder.Services.AddSingleton<ICache, RedisCache>();

var app = builder.Build();

await app.ConfigureDatabaseAsync<AppDbContext>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();