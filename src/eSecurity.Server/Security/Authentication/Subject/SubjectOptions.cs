namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class SubjectOptions
{
    public int PublicSubjectLength { get; set; }
    public string PairwiseSubjectSalt { get; set; } = string.Empty;
}