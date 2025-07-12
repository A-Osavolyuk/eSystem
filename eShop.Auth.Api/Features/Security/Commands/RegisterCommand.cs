using eShop.Domain.Abstraction.Messaging.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record RegisterCommand(RegistrationRequest Request) : IRequest<Result>;

public sealed class RegisterCommandHandler(
    IPermissionManager permissionManager,
    IUserManager userManager,
    IMessageService messageService,
    ICodeManager codeManager,
    IRoleManager roleManager) : IRequestHandler<RegisterCommand, Result>
{
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IRoleManager roleManager = roleManager;

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

        var role = await roleManager.FindByNameAsync("User", cancellationToken);

        if (role is null)
        {
            return Results.NotFound("Cannot find role with name User");
        }
            
        var assignRoleResult = await roleManager.AssignAsync(newUser, role, cancellationToken);

        if (!assignRoleResult.Succeeded)
        {
            return assignRoleResult;
        }

        var permissions = role.Permissions.Select(x => x.Permission).ToList();
            
        var grantPermissionsResult = await permissionManager.GrantAsync(newUser, permissions, cancellationToken);

        if (!grantPermissionsResult.Succeeded)
        {
            return grantPermissionsResult;
        }
        
        user = await userManager.FindByEmailAsync(request.Request.Email, cancellationToken);

        var code = await codeManager.GenerateAsync(user!, SenderType.Email, CodeType.Verify, cancellationToken);
        
        await messageService.SendMessageAsync(SenderType.Email, "email-verify", new { Code = code, },
            new EmailCredentials()
            {
                To = request.Request.Email,
                Subject = "Email verification",
                UserName = newUser.UserName!
            }, cancellationToken);

        return Result.Success($"Your account have been successfully registered. " +
                              $"Now you have to confirm you email address to log in. " +
                              $"We have sent an email with instructions to your email address.");
    }
}