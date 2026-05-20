using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using eSecurity.Client;
using eSecurity.Client.Common.Configurations;
using eSecurity.Client.Common.Confirmation;
using eSecurity.Client.Common.Http;
using eSecurity.Client.Common.JS;
using eSecurity.Client.Common.State;
using eSecurity.Client.Security;
using eSecurity.Client.Security.Authentication;
using eSecurity.Client.Security.Authentication.OpenIdConnect;
using eSecurity.Client.Security.Authentication.Password;
using eSecurity.Client.Security.Authentication.TwoFactor;
using eSecurity.Client.Security.Authorization.Consent;
using eSecurity.Client.Security.Authorization.Devices;
using eSecurity.Client.Security.Authorization.LinkedAccounts;
using eSecurity.Client.Security.Authorization.Verification;
using eSecurity.Client.Security.Credentials.PublicKey;
using eSecurity.Client.Security.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
builder.Services.Configure<BackendOptions>(builder.Configuration.GetSection("Backend"));
builder.Services.AddScoped<UserState>();
builder.Services.AddScoped<SignInManager>();
builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
{
    var gatewayUrl = builder.Configuration.GetValue<string>("Backend:Uri");
    client.BaseAddress = new Uri(gatewayUrl ?? throw new NullReferenceException("BFF URI is empty."));
});

builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IPasskeyService, PasskeyService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IConsentService, ConsentService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<ILinkedAccountService, LinkedAccountService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUsernameService, UsernameService>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
builder.Services.AddScoped<IConnectService, ConnectService>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

builder.Services.AddMudExtensions();
builder.Services.AddJs();
builder.Services.AddConfirmation();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, SecurityStateProvider>();

await builder.Build().RunAsync();