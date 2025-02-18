namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record SetPersonalDataCommand(SetPersonalDataRequest Request)
    : IRequest<Result<SetPersonalDataResponse>>;

internal sealed record SetPersonalDataCommandHandler(
    AppManager appManager) : IRequestHandler<SetPersonalDataCommand, Result<SetPersonalDataResponse>>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result<SetPersonalDataResponse>> Handle(SetPersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email: {request.Request.Email}"));
        }

        var entity = Mapper.ToPersonalDataEntity(request.Request);
        var result = await appManager.ProfileManager.SetPersonalDataAsync(user, entity);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Failed to setting personal data with message: {result.Errors.First().Description}"));
        }

        return new(new SetPersonalDataResponse() { Message = "Personal data was successfully set" });
    }
}