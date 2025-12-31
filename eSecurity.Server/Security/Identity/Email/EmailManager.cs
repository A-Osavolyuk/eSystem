using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Email;

public class EmailManager(AuthDbContext context) : IEmailManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken)
    {
        return await _context.UserEmails
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, EmailType type,
        CancellationToken cancellationToken)
    {
        return await _context.UserEmails
            .Where(x => x.UserId == user.Id && x.Type == type)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> FindByTypeAsync(UserEntity user, EmailType type,
        CancellationToken cancellationToken)
    {
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Type == type, cancellationToken);
    }

    public async ValueTask<UserEmailEntity?> FindByEmailAsync(UserEntity user, string email,
        CancellationToken cancellationToken)
    {
        return await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Email == email, cancellationToken);
    }

    public async ValueTask<Result> SetAsync(UserEntity user, string email, EmailType type,
        CancellationToken cancellationToken = default)
    {
        var userEmail = new UserEmailEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            Type = type,
            IsVerified = true,
            VerifiedDate = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };

        await _context.UserEmails.AddAsync(userEmail, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default)
    {
        var userEmail = await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Email == currentEmail, cancellationToken);

        if (userEmail is null) return Results.NotFound($"User doesn't have email {currentEmail}");

        userEmail.Email = newEmail;
        userEmail.NormalizedEmail = newEmail.ToUpperInvariant();
        userEmail.IsVerified = true;
        userEmail.VerifiedDate = DateTimeOffset.UtcNow;
        userEmail.UpdateDate = DateTimeOffset.UtcNow;

        _context.UserEmails.Update(userEmail);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ManageAsync(UserEntity user, EmailType type, string email,
        CancellationToken cancellationToken = default)
    {
        var currentEmail = await _context.UserEmails.FirstOrDefaultAsync(
            x => x.Type == type && x.UserId == user.Id, cancellationToken);

        if (currentEmail is not null)
        {
            currentEmail.Type = EmailType.Secondary;
            currentEmail.UpdateDate = DateTimeOffset.UtcNow;
            _context.UserEmails.Update(currentEmail);
        }

        var nextEmail = await _context.UserEmails.FirstOrDefaultAsync(
            x => x.Email == email && x.UserId == user.Id, cancellationToken);

        if (nextEmail is null) return Results.BadRequest($"User doesn't have email {email}.");

        nextEmail.Type = type;
        nextEmail.UpdateDate = DateTimeOffset.UtcNow;

        _context.UserEmails.Update(nextEmail);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default)
    {
        var userEmail = await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Email == email, cancellationToken);

        if (userEmail == null) return Results.NotFound($"User doesn't have email {email}");

        userEmail.IsVerified = true;
        userEmail.VerifiedDate = DateTimeOffset.UtcNow;
        userEmail.CreateDate = DateTimeOffset.UtcNow;

        _context.UserEmails.Update(userEmail);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ResetAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default)
    {
        var userEmail = await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Email == currentEmail, cancellationToken);

        if (userEmail is null) return Results.NotFound($"User doesn't have email {currentEmail}");

        userEmail.Email = newEmail;
        userEmail.NormalizedEmail = newEmail.ToUpperInvariant();
        userEmail.IsVerified = true;
        userEmail.VerifiedDate = DateTimeOffset.UtcNow;
        userEmail.UpdateDate = DateTimeOffset.UtcNow;

        _context.UserEmails.Update(userEmail);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default)
    {
        var userEmail = await _context.UserEmails.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Email == email, cancellationToken);

        if (userEmail is null) return Results.BadRequest("Invalid email");

        _context.UserEmails.Remove(userEmail);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> AddAsync(UserEntity user, string email, EmailType type,
        CancellationToken cancellationToken = default)
    {
        if (await _context.UserEmails.AnyAsync(x => x.Email == email, cancellationToken))
            return Results.BadRequest("Email is already taken");

        var userEmail = new UserEmailEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            Type = type,
            CreateDate = DateTimeOffset.UtcNow
        };

        await _context.UserEmails.AddAsync(userEmail, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> IsTakenAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.ToUpperInvariant();
        return await _context.UserEmails.AnyAsync(
            u => u.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public async ValueTask<bool> HasAsync(UserEntity user, EmailType type, CancellationToken cancellationToken = default)
        => await _context.UserEmails.AnyAsync(x => x.UserId == user.Id && x.Type == type, cancellationToken);
}