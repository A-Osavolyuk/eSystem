using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Phone;

public class PhoneManager(AuthDbContext context) : IPhoneManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserPhoneNumberEntity>> GetAllAsync(UserEntity user,
        CancellationToken cancellationToken)
    {
        return await _context.UserPhoneNumbers
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<UserPhoneNumberEntity>> GetAllAsync(UserEntity user, PhoneNumberType type,
        CancellationToken cancellationToken)
    {
        return await _context.UserPhoneNumbers
            .Where(x => x.UserId == user.Id && x.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserPhoneNumberEntity?> FindByTypeAsync(UserEntity user, PhoneNumberType type,
        CancellationToken cancellationToken)
    {
        return await _context.UserPhoneNumbers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Type == type, cancellationToken);
    }

    public async ValueTask<UserPhoneNumberEntity?> FindByPhoneAsync(UserEntity user, string phone,
        CancellationToken cancellationToken)
    {
        return await _context.UserPhoneNumbers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.PhoneNumber == phone, cancellationToken);
    }

    public async ValueTask<Result> SetAsync(UserEntity user, string phoneNumber, PhoneNumberType type,
        CancellationToken cancellationToken = default)
    {
        if (await ExistsAsync(phoneNumber, cancellationToken))
            return Results.BadRequest("Phone number is already taken");

        var userPhoneNumber = new UserPhoneNumberEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            PhoneNumber = phoneNumber,
            Type = type,
            IsVerified = true,
            VerifiedDate = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };

        await _context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = await _context.UserPhoneNumbers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.PhoneNumber == phoneNumber, cancellationToken);

        if (userPhoneNumber == null) return Results.NotFound("Phone number not found");

        userPhoneNumber.IsVerified = true;
        userPhoneNumber.VerifiedDate = DateTimeOffset.UtcNow;
        userPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;

        _context.UserPhoneNumbers.Update(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ResetAsync(UserEntity user, string currentEmail, string newPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = await _context.UserPhoneNumbers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.PhoneNumber == currentEmail, cancellationToken);

        if (userPhoneNumber is null) return Results.NotFound("Phone number not found");

        userPhoneNumber.PhoneNumber = newPhoneNumber;
        userPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;
        userPhoneNumber.IsVerified = true;
        userPhoneNumber.VerifiedDate = DateTimeOffset.UtcNow;

        _context.UserPhoneNumbers.Update(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, string phoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = await _context.UserPhoneNumbers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.PhoneNumber == phoneNumber, cancellationToken);

        if (userPhoneNumber == null) return Results.NotFound("Phone number not found");

        _context.UserPhoneNumbers.Remove(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string currentPhoneNumber,
        string newPhoneNumber,
        CancellationToken cancellationToken = default)
    {
        var userPhoneNumber = await _context.UserPhoneNumbers.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.PhoneNumber == currentPhoneNumber, cancellationToken);

        if (userPhoneNumber is null) return Results.NotFound("Phone number not found");

        userPhoneNumber.PhoneNumber = newPhoneNumber;
        userPhoneNumber.UpdateDate = DateTimeOffset.UtcNow;
        userPhoneNumber.IsVerified = true;
        userPhoneNumber.VerifiedDate = DateTimeOffset.UtcNow;

        _context.UserPhoneNumbers.Update(userPhoneNumber);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> AddAsync(UserEntity user, string phoneNumber, PhoneNumberType type,
        CancellationToken cancellationToken = default)
    {
        if (await ExistsAsync(phoneNumber, cancellationToken))
            return Results.BadRequest("Phone number is already taken");

        var userPhoneNumber = new UserPhoneNumberEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = type,
            PhoneNumber = phoneNumber,
            CreateDate = DateTimeOffset.UtcNow
        };

        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.UserPhoneNumbers.AddAsync(userPhoneNumber, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> IsTakenAsync(string phoneNumber,
        CancellationToken cancellationToken = default) => await ExistsAsync(phoneNumber, cancellationToken);

    private async ValueTask<bool> ExistsAsync(string phoneNumber, CancellationToken cancellationToken = default)
        => await _context.UserPhoneNumbers.AnyAsync(x => x.PhoneNumber == phoneNumber, cancellationToken);
}