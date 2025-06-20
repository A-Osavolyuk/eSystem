using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.PersonalData.Commands;

public sealed record ChangePersonalDataCommand(ChangePersonalDataRequest Request) : IRequest<Result>;

public sealed class UpdatePersonalDataCommandHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<ChangePersonalDataCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(ChangePersonalDataCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.Id, cancellationToken);

        if (user is null)
        {
            return Result.Failure(new Error()
            {
                Code = ErrorCode.NotFound,
                Message = "Not found",
                Details = $"Cannot find user with ID {request.Request.Id}."
            });
        }
        
        var personalData = await personalDataManager.FindAsync(user, cancellationToken);

        if (personalData is null)
        {
            return Results.NotFound($"Cannot find personal data of user with ID {request.Request.Id}");
        }
        
        personalData.FirstName = request.Request.FirstName;
        personalData.LastName = request.Request.LastName;
        personalData.Gender = request.Request.Gender;
        personalData.DateOfBirth = request.Request.BirthDate!.Value;
        
        var result = await personalDataManager.UpdateAsync(personalData, cancellationToken);

        return result;
    }
}