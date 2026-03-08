using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Verification;

public class VerificationManager(AuthDbContext context) : IVerificationManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<VerificationRequestEntity?> FindByIdAsync(Guid id, 
        CancellationToken cancellationToken = default)
    {
        return await _context.VerificationRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default)
    {
        await _context.VerificationRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> ConsumeAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default)
    {
        request.ConsumedAt = DateTimeOffset.UtcNow;
        request.Status = VerificationStatus.Consumed;
        
        _context.VerificationRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> CancelAsync(VerificationRequestEntity request, 
        CancellationToken cancellationToken = default)
    {
        request.CancelledAt = DateTimeOffset.UtcNow;
        request.Status = VerificationStatus.Cancelled;
        
        _context.VerificationRequests.Update(request);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}