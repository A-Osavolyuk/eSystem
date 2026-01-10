namespace eSecurity.Server.Security.Identity.Options;

public class PasswordOptions
{
    public int RequiredLength { get; set; } = 6;
    public bool RequireUppercase { get; set; } = false;
    public int RequiredUppercase { get; set; } = 1;
    public bool RequireLowercase { get; set; } = false;
    public int RequiredLowercase { get; set; } = 1;
    public bool RequireDigit { get; set; } = false;
    public int RequiredDigits { get; set; } = 1;
    public bool RequireNonAlphanumeric { get; set; } = false;
    public int RequiredNonAlphanumeric { get; set; } = 1;
    public bool RequireUniqueChars { get; set; } = false;
    public int RequiredUniqueChars { get; set; } = 1;
}