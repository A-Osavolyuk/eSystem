namespace eShop.Domain.DTOs;

public class UserTwoFactorMethod
{
    public required TwoFactorMethod Method { get; set; }
    public required bool Preferred { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}