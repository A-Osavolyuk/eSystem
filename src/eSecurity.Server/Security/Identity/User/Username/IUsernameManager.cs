using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Identity.User.Username;

public interface IUsernameManager
{
    public ValueTask<Result> SetAsync(UserEntity user, string username, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ChangeAsync(UserEntity user, string username,
        CancellationToken cancellationToken = default);
    
    public ValueTask<bool> IsTakenAsync(string username, CancellationToken cancellationToken = default);
}