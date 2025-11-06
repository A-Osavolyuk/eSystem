using eSecurity.Security.Identity.Privacy;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Users.Commands;

public record AddPersonalDataCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender Gender { get; set; }
    public DateTime BirthDate { get; set; }
}

public class AddPersonalDataCommandHandler(
    IUserManager userManager, 
    IPersonalDataManager personalDataManager) : IRequestHandler<AddPersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(AddPersonalDataCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");

        var entity = Mapper.Map(request);
        
        var result = await personalDataManager.CreateAsync(entity, cancellationToken);
        if(!result.Succeeded) return result;
        
        user.PersonalDataId = entity.Id;
        
        var userResult = await userManager.UpdateAsync(user, cancellationToken);
        return userResult;
    }
}