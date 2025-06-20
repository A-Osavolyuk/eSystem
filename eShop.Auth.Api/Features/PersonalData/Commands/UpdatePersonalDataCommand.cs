using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.PersonalData.Commands;

public sealed record UpdatePersonalDataCommand(ChangePersonalDataRequest Request) : IRequest<Result>;

public sealed class UpdatePersonalDataCommandHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<UpdatePersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(UpdatePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with ID {request.Request.Id}."
            });
        }

        var entity = Mapper.Map(request.Request);
        var result = await personalDataManager.UpdateAsync(entity, cancellationToken);

        return result;
    }
}