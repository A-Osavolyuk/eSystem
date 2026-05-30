namespace eSecurity.Idp.Common.Messaging.Sms.Builders;

public sealed class CodeVerificationSmsContext : SmsContext
{
    public required string Code { get; set; }
}

public sealed class CodeVerificationSmsBuilder : ISmsBuilder<CodeVerificationSmsContext>
{
    public string Build(CodeVerificationSmsContext context)
    {
        return $"Your verification code {context.Code}";
    }
}