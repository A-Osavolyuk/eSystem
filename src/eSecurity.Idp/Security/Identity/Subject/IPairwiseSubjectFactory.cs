namespace eSecurity.Idp.Security.Identity.Subject;

public interface IPairwiseSubjectFactory
{
    public string CreateSubject(string userIdentifier, string sectorIdentifier, string salt);
}