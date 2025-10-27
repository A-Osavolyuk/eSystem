// Global using directives

global using System.IdentityModel.Tokens.Jwt;
global using System.Net.Http.Headers;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;
global using Blazored.LocalStorage;
global using eAccount.Domain.Interfaces;
global using eSystem.Core.Common.Http;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Components.Authorization;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using AuthenticationManager = eAccount.Infrastructure.Security.Authentication.AuthenticationManager;