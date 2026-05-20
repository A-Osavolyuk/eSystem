using eSecurity.Idp.Security.Cryptography.Keys;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public sealed class PublicSubjectFactoryContext : SubjectFactoryContext
{
    public int Length { get; set; }
}

public class PublicSubjectFactory(IKeyFactory keyFactory) : ISubjectFactory<PublicSubjectFactoryContext>
{
    private readonly IKeyFactory _keyFactory = keyFactory;

    public string CreateSubject(PublicSubjectFactoryContext factoryContext)
        => _keyFactory.Create(factoryContext.Length);
}