using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class SigningCertificateEntity : Entity
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; }
    public byte[] ProtectedCertificate { get; set; } = null!;
    public byte[] ProtectedPassword { get; set; } = null!;

    public DateTimeOffset ExpireDate { get; set; }
    public DateTimeOffset RotateDate { get; set; }
}