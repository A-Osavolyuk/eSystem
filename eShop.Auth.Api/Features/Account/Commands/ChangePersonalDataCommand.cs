namespace eShop.Auth.Api.Features.Account.Commands;

internal sealed record ChangePersonalDataCommand(ChangePersonalDataRequest Request)
    : IRequest<Result<ChangePersonalDataResponse>>;

internal sealed class ChangePersonalDataCommandHandler(
    AppManager appManager,
    AuthDbContext context) : IRequestHandler<ChangePersonalDataCommand, Result<ChangePersonalDataResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly AuthDbContext context = context;

    public async Task<Result<ChangePersonalDataResponse>> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is null)
        {
            return new(new NotFoundException($"Cannot find user with email {request.Request.Email}."));
        }

        var entity = Mapper.ToPersonalDataEntity(request.Request);
        var result = await appManager.ProfileManager.ChangePersonalDataAsync(user, entity);

        if (!result.Succeeded)
        {
            return new Result<ChangePersonalDataResponse>(new FailedOperationException(
                $"Failed on changing personal data with message: {result.Errors.First().Description}"));
        }

        return new(new ChangePersonalDataResponse()
        {
            Message = "Personal data was successfully updated"
        });
    }
}