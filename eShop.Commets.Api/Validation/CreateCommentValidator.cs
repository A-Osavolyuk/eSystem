using eShop.Commets.Api.Commands.Comments;

namespace eShop.Commets.Api.Validation;

internal sealed class CreateCommentValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Request).SetValidator(new Application.Validation.Comments.CreateCommentValidator());
    }
}