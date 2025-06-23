using eShop.Domain.DTOs;

namespace eShop.BlazorWebUI.Models;

public class SecurityModel
{
    public List<UserProviderDto> Providers { get; set; } = [];
}