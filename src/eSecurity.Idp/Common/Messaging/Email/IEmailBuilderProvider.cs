namespace eSecurity.Idp.Common.Messaging.Email;

public interface IEmailBuilderProvider
{
    public IEmailBuilder<TContext> GetBuilder<TContext>() 
        where TContext : EmailContext;
}