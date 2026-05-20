using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class SigningCertificateEntity : Entity
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; }
    public byte[] ProtectedCertificate { get; set; } = null!;
    public byte[] ProtectedPassword { get; set; } = null!;

    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset? RotatedAt { get; set; }
}