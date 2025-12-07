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
}