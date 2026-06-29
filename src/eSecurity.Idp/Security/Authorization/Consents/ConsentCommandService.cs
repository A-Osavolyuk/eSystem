using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Client;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;

namespace eSecurity.Idp.Security.Authorization.Consents;

public sealed class ConsentCommandService(
    AuthDbContext context,
    IClientQueryService clientQuery,
    IConsentQueryService consentQuery,
    IOptions<OpenIdConfiguration> options) : IConsentCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly IClientQueryService _clientQuery = clientQuery;
    private readonly IConsentQueryService _consentQuery = consentQuery;
    private readonly OpenIdConfiguration _options = options.Value;

    public async ValueTask<Result> CreateAsync(ConsentEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.Consents.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> GrantScopesAsync(Guid consentId, IEnumerable<string> scopes,
        CancellationToken cancellationToken = default)
    {
        var consentEntity = await _consentQuery.GetByIdAsync(consentId, cancellationToken);
        if (consentEntity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid consent"
            });
        }
        
        var scopeEntities = await _clientQuery.GetAllowedScopesAsync(consentEntity.ClientId, cancellationToken);
        foreach (var scope in scopes)
        {
            if (!_options.ScopesSupported.Contains(scope))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidScope,
                    Description = $"'{scope}' scope is not supported"
                });
            }
            
            var scopeEntity = scopeEntities.FirstOrDefault(x => x.Scope.Value == scope);
            if (scopeEntity is null)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidScope,
                    Description = $"'{scope}' is not supported by client"
                });
            }
            
            consentEntity.GrantedScopes.Add(new GrantedScopeEntity()
            {
                Id = Guid.CreateVersion7(),
                ConsentId = consentEntity.Id,
                ClientScopeId = scopeEntity.Id
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}