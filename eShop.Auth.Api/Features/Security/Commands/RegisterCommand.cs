using eShop.Domain.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

internal sealed record RegisterCommand(RegistrationRequest Request) : IRequest<Result>;

internal sealed class RegisterCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    IConfiguration configuration,
    ICodeManager codeManager) : IRequestHandler<RegisterCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;
    private readonly string frontendUri = configuration["Configuration:General:Frontend:Clients:BlazorServer:Uri"]!;
    private readonly string defaultRole = configuration["Configuration:General:DefaultValues:DefaultRole"]!;
    private readonly string defaultPermission = configuration["Configuration:General:DefaultValues:DefaultPermission"]!;

    public async Task<Result> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        if (user is not null)
        {
            return Results.NotFound("User already exists");
        }

        var newUser = Mapper.Map(request.Request);
        var registrationResult = await userManager.CreateAsync(newUser, request.Request.Password, cancellationToken);

        if (!registrationResult.Succeeded)
        {
            return registrationResult;
        }

        var assignDefaultRoleResult = await userManager.AssignRoleAsync(newUser, defaultRole, cancellationToken);

        if (!assignDefaultRoleResult.Succeeded)
        {
            return assignDefaultRoleResult;
        }

        var permission = await permissionManager.FindByNameAsync(defaultPermission, cancellationToken);

        if (permission is null)
        {
            return Results.NotFound($"Cannot find permission with name {defaultPermission}");       
        }
        
        var issuingPermissionsResult = await permissionManager.GrantAsync(newUser, permission, cancellationToken);

        if (!issuingPermissionsResult.Succeeded)
        {
            return issuingPermissionsResult;
        }
        
        user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        var code = await codeManager.GenerateAsync(user!, Verification.Verify, cancellationToken);
        
        await messageService.SendMessageAsync("email:email-verification", new EmailVerificationMessage()
        {
            To = request.Request.Email,
            Subject = "Email verification",
            Code = code,
            UserName = newUser.UserName!
        }, cancellationToken);

        return Result.Success($"Your account have been successfully registered. " +
                              $"Now you have to confirm you email address to log in. " +
                              $"We have sent an email with instructions to your email address.");
    }
}