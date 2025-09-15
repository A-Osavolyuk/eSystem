// Global using directives

global using System.IdentityModel.Tokens.Jwt;
global using System.Net.Http.Headers;
global using System.Security.Claims;
global using System.Text;
global using Blazored.LocalStorage;
global using eShop.Application.Extensions;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using AuthenticationManager = eShop.Blazor.Infrastructure.Security.AuthenticationManager;
global using HttpRequest = eShop.Domain.Common.Http.HttpRequest;
global using HttpResponse = eShop.Domain.Common.Http.HttpResponse;