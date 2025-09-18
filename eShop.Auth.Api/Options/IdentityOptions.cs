namespace eShop.Auth.Api.Options;

public class IdentityOptions
{
    public AccountOptions Account { get; set; } = new();
    public SignInOptions SignIn { get; set; } = new();
    public PasswordOptions Password { get; set; } = new();
    public CredentialOptions Credentials { get; set; } = new();
    public CodeOptions Code { get; set; } = new();
}