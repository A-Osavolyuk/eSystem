namespace eShop.Auth.Api.Options;

public class IdentityOptions
{
    public AccountOptions Account { get; set; } = new AccountOptions();
    public SignInOptions SignIn { get; set; } = new SignInOptions();
    public PasswordOptions Password { get; set; } = new PasswordOptions();
}