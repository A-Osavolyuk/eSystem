using eSecurity.Server.Security.Cryptography.Keys;

namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class PublicSubjectContext : SubjectContext
{
    public int Length { get; set; }
}

public class PublicSubjectFactory(IKeyFactory keyFactory) : ISubjectFactory<PublicSubjectContext>
{
    private readonly IKeyFactory _keyFactory = keyFactory;

    public string CreateSubject(PublicSubjectContext context)
        => _keyFactory.Create(context.Length);
}