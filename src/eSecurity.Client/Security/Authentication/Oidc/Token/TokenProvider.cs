using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSystem.Core.Common.Cache.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;

namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public class TokenProvider(
    IDataProtectionProvider protectionProvider,
    ICacheService cacheService) : ITokenProvider
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Token);

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var protectedValue = await _cacheService.GetAsync<string>(key, cancellationToken);
        return string.IsNullOrEmpty(protectedValue) ? null : _protector.Unprotect(protectedValue);
    }
        

    public async Task SetAsync(string key, string token, TimeSpan timeStamp,
        CancellationToken cancellationToken = default)
    {
        var protectedValue = _protector.Protect(token);
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(timeStamp);
        await _cacheService.SetAsync(key, protectedValue, options, cancellationToken);
    }
}