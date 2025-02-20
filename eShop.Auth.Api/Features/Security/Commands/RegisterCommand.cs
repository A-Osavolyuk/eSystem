namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record RegisterCommand(RegistrationRequest Request) : IRequest<Result<RegistrationResponse>>;

internal sealed class RegisterCommandHandler(
    AppManager appManager,
    IMessageService messageService,
    IConfiguration configuration) : IRequestHandler<RegisterCommand, Result<RegistrationResponse>>
{
    private readonly AppManager appManager = appManager;
    private readonly IMessageService messageService = messageService;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;
    private readonly string defaultPermission = configuration["Configuration:General:DefaultValues:DefaultPermission"]!;

    public async Task<Result<RegistrationResponse>> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Request.Email);

        if (user is not null)
        {
            return new(new BadRequestException("User already exists"));
        }

        var newUser = Mapper.ToAppUser(request.Request);
        var registrationResult = await appManager.UserManager.CreateAsync(newUser, request.Request.Password);

        if (!registrationResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot create user due to server error: {registrationResult.Errors.First().Description}"));
        }

        var assignDefaultRoleResult = await appManager.UserManager.AddToRoleAsync(newUser, defaultRole);

        if (!assignDefaultRoleResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot assign role {defaultRole} to user with email {newUser.Email} " +
                $"due to server errors: {assignDefaultRoleResult.Errors.First().Description}"));
        }

        var issuingPermissionsResult =
            await appManager.PermissionManager.IssuePermissionsAsync(newUser, [defaultPermission]);

        if (!issuingPermissionsResult.Succeeded)
        {
            return new(new FailedOperationException(
                $"Cannot issue permissions for user with email {request.Request.Email} " +
                $"due to server errors: {issuingPermissionsResult.Errors.First().Description}"));
        }

        var code = await appManager.SecurityManager.GenerateVerificationCodeAsync(newUser.Email!,
            VerificationCodeType.VerifyEmail);

        await messageService.SendMessageAsync("email-verification", new EmailVerificationMessage()
        {
            To = request.Request.Email,
            Subject = "Email verification",
            Code = code,
            UserName = newUser.UserName!
        });


        return new(new RegistrationResponse()
        {
            Message = $"Your account have been successfully registered. " +
                      $"Now you have to confirm you email address to log in. " +
                      $"We have sent an email with instructions to your email address."
        });
    }
}