namespace eSecurity.Server.Security.Identity.Claims.Subject;

public interface IPairwiseSubjectFactory
{
    public string CreateSubject(string userIdentifier, string sectorIdentifier, string salt);
}