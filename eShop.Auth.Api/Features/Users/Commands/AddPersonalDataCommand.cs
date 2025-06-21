using eShop.Domain.Requests.API.Auth;

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
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.Id}");
        }

        var entity = Mapper.Map(request.Request);
        
        var result = await personalDataManager.CreateAsync(entity, cancellationToken);

        return result;
    }
}