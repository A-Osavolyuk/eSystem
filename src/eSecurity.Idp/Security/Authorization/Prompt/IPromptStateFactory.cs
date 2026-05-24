namespace eSecurity.Idp.Security.Authorization.Prompt;

public interface IPromptStateFactory
{
    public string CreateState(PromptContext context);
}