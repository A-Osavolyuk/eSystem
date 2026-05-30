namespace eSecurity.Idp.Common.Messaging.Email;

public interface IEmailBuilder<in TContext> where TContext : EmailContext
{
    public string Build(TContext context);
}