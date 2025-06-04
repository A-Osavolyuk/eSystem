using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record SetPersonalDataCommand(SetPersonalDataRequest Request)
    : IRequest<Result>;

internal sealed record SetPersonalDataCommandHandler(
    IPersonalDataManager personalDataManager,
    IUserManager userManager) : IRequestHandler<SetPersonalDataCommand, Result>
{
    private readonly IPersonalDataManager personalDataManager = personalDataManager;
    private readonly IUserManager userManager = userManager;
    public async Task<Result> Handle(SetPersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email: {request.Request.Email}");
        }

        var entity = Mapper.Map(request.Request);
        var result = await personalDataManager.CreateAsync(user, entity, cancellationToken);

        return result;
    }
}