using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public interface IPublicSubjectCommandService
{
    ValueTask<Result> CreateAsync(PublicSubjectEntity entity, CancellationToken cancellationToken = default);
}