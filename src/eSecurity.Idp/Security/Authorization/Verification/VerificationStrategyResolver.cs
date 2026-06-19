using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Idp.Security.Authorization.Verification;

public sealed class VerificationStrategyResolver(IServiceProvider serviceProvider) : IVerificationStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IVerificationStrategy Resolve(VerificationMethod verificationMethod)
        => _serviceProvider.GetRequiredKeyedService<IVerificationStrategy>(verificationMethod);
}