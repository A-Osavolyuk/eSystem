using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Identity;

public interface IEmailService
{
    public ValueTask<HttpResponse> AddEmailAsync(AddEmailRequest request);
    public ValueTask<HttpResponse> CheckEmailAsync(CheckEmailRequest request);
    public ValueTask<HttpResponse> ChangeEmailAsync(ChangeEmailRequest request);
    public ValueTask<HttpResponse> VerifyEmailAsync(VerifyEmailRequest request);
    public ValueTask<HttpResponse> ManageEmailAsync(ManageEmailRequest request);
    public ValueTask<HttpResponse> RemoveEmailAsync(RemoveEmailRequest request);
    public ValueTask<HttpResponse> ResetEmailAsync(ResetEmailRequest request);
}