using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Users.Commands;

public record AddPersonalDataCommand(AddPersonalDataRequest Request) : IRequest<Result>;

public class AddPersonalDataCommandHandler(
    IUserManager userManager, 
    IPersonalDataManager personalDataManager) : IRequestHandler<AddPersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(AddPersonalDataCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        var entity = Mapper.Map(request.Request);
        
        var result = await personalDataManager.CreateAsync(entity, cancellationToken);
        if(!result.Succeeded) return result;
        
        user.PersonalDataId = entity.Id;
        
        var userResult = await userManager.UpdateAsync(user, cancellationToken);
        return userResult;
    }
}