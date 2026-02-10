using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Ciba;

public interface ICibaRequestManager
{
    public ValueTask<CibaRequestEntity?> FindByAuthReqIdAsync(string authReqId,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(CibaRequestEntity entity, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UpdateAsync(CibaRequestEntity entity, 
        CancellationToken cancellationToken = default);
}