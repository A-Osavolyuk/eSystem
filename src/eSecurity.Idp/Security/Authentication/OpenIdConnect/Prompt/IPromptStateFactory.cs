namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;

public interface IPromptStateFactory
{
    public string CreateState(PromptContext context);
}