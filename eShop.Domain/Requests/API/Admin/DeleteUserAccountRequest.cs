namespace eShop.Domain.Requests.API.Admin;

public record class DeleteUserAccountRequest
{
    public Guid UserId { get; set; }
}