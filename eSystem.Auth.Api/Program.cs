using eSystem.Auth.Api;
using eSystem.Auth.Api.Messaging;
using eSystem.Auth.Api.Security;
using eSystem.Auth.Api.Storage;
using eSystem.Core.Common.Cache.Redis;
using eSystem.Core.Common.Documentation;
using eSystem.Core.Common.Errors;
using eSystem.Core.Common.Logging;
using eSystem.Core.Common.Versioning;
using eSystem.Core.Data;
using eSystem.Core.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.AddVersioning();
builder.AddMessaging();
builder.AddValidation<IAssemblyMarker>();
builder.AddServiceDefaults();
builder.AddSecurity();
builder.AddRedisCache();
builder.AddMsSqlDb();
builder.AddLogging();
builder.AddExceptionHandler();
builder.AddDocumentation();
builder.AddStorage();

builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers().AddJsonOptions(cfg => cfg.JsonSerializerOptions.WriteIndented = true);
builder.Services.AddMediatR(
    cfg => cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseExceptionHandler();

await app.ConfigureDatabaseAsync<AuthDbContext>();

app.Run();