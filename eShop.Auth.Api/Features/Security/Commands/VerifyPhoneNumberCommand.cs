namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request)
    : IRequest<Result<VerifyPhoneNumberResponse>>;

internal sealed class VerifyPhoneNumberCommandHandler(
    AppManager appManager,
    IMessageService messageService) : IRequestHandler<VerifyPhoneNumberCommand, Result<VerifyPhoneNumberResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result<VerifyPhoneNumberResponse>> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.PhoneNumber);

        if (user is null)
        {
            return new Result<VerifyPhoneNumberResponse>(
                new NotFoundException($"Cannot find user with phone number ${request.Request.PhoneNumber}"));
        }

        var result = await appManager.SecurityManager.VerifyPhoneNumberAsync(user, request.Request.Code);

        if (!result.Succeeded)
        {
            return new(new FailedOperationException(
                $"Failed on verifying phone number with message: {result.Errors.First().Description}"));
        }

        return new Result<VerifyPhoneNumberResponse>(new VerifyPhoneNumberResponse()
        {
            Message = "Phone number was successfully verified"
        });
    }
}