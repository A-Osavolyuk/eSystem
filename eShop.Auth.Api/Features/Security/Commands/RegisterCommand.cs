using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

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
        var isEmailTaken = await userManager.IsEmailTakenAsync(request.Request.Email, cancellationToken);

        if (isEmailTaken)
        {
            return Results.NotFound("User with same email address already exists");
        }
        
        var isUserNameTaken = await userManager.IsUserNameTakenAsync(request.Request.UserName, cancellationToken);
        
        if (isUserNameTaken)
        {
            return Results.NotFound("Username is already taken");
        }

        var user = Mapper.Map(request.Request);
        
        var registrationResult = await userManager.CreateAsync(user, request.Request.Password, cancellationToken);

        if (!registrationResult.Succeeded)
        {
            return registrationResult;
        }

        var role = await roleManager.FindByNameAsync("User", cancellationToken);

        if (role is null)
        {
            return Results.NotFound("Cannot find role with name User");
        }
            
        var assignRoleResult = await roleManager.AssignAsync(user, role, cancellationToken);

        if (!assignRoleResult.Succeeded)
        {
            return assignRoleResult;
        }

        var permissions = role.Permissions.Select(x => x.Permission).ToList();

        foreach (var permission in permissions)
        {
            var grantResult = await permissionManager.GrantAsync(user, permission, cancellationToken);

            if (!grantResult.Succeeded)
            {
                return grantResult;
            }
        }

        var code = await codeManager.GenerateAsync(user!, SenderType.Email, CodeType.Verify, 
            CodeResource.Email, cancellationToken);
        
        var message = new VerifyEmailMessage()
        {
            Credentials = new ()
            {
                { "To", user!.Email },
                { "Subject", "Email verification" }
            },
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        var response = new RegistrationResponse()
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
        };

        return Result.Success(response, "Your account have been successfully registered.");
    }
}