using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Ciba;

public interface ICibaRequestManager
{
    public ValueTask<CibaRequestEntity?> FindByAuthReqIdAsync(string authReqId,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(CibaRequestEntity entity, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UpdateAsync(CibaRequestEntity entity, 
        CancellationToken cancellationToken = default);
}