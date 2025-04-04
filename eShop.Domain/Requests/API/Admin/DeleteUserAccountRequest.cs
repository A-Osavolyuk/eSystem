namespace eShop.Domain.Requests.Api.Admin;

public record class DeleteUserAccountRequest
{
    public Guid UserId { get; set; }
}