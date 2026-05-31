using eSecurity.Idp.Security.Cryptography.Keys;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public sealed class PublicSubjectFactoryContext : SubjectFactoryContext
{
    public int Length { get; set; }
}

public class PublicSubjectFactory : ISubjectFactory<PublicSubjectFactoryContext>
{
    public string CreateSubject(PublicSubjectFactoryContext factoryContext)
        => RandomKeyFactory.Create(factoryContext.Length);
}