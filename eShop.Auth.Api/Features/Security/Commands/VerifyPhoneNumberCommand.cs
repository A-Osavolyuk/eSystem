namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request)
    : IRequest<Result>;

internal sealed class VerifyPhoneNumberCommandHandler(
    AppManager appManager,
    IMessageService messageService) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByPhoneNumberAsync(request.Request.PhoneNumber);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with phone number ${request.Request.PhoneNumber}"
            });
        }

        var result = await appManager.SecurityManager.VerifyPhoneNumberAsync(user, request.Request.Code);

        if (!result.Succeeded)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.InternalServerError,
                Message = "Internal server error",
                Details = $"Failed on verifying phone number with message: {result.Errors.First().Description}"
            });
        }

        return Result.Success("Phone number was successfully verified");
    }
}