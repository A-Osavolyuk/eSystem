using eSecurity.Security.Identity.Privacy;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Users.Commands;

public sealed record ChangePersonalDataCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public Gender Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}

public sealed class UpdatePersonalDataCommandHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<ChangePersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");
        if (user.PersonalData is null) return Results.NotFound(
            $"Cannot find personal data of user with ID {request.UserId}");
        
        user.PersonalData.FirstName = request.FirstName;
        user.PersonalData.LastName = request.LastName;
        user.PersonalData.MiddleName = request.MiddleName;
        user.PersonalData.Gender = request.Gender;
        user.PersonalData.BirthDate = request.BirthDate!.Value;
        user.PersonalData.UpdateDate = DateTimeOffset.UtcNow;
        
        var result = await personalDataManager.UpdateAsync(user.PersonalData, cancellationToken);

        return result;
    }
}