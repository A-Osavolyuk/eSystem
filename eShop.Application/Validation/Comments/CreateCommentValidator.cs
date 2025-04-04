using eShop.Domain.Requests.Api.Comments;

namespace eShop.Application.Validation.Comments;

public class CreateCommentValidator : Validator<CreateCommentRequest>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");
        
        RuleFor(x => x.CommentText)
            .NotEmpty().WithMessage("Comment text is required")
            .MinimumLength(8).WithMessage("Comment text must be at least 8 characters")
            .MaximumLength(3000).WithMessage("Comment text cannot exceed 3000 characters");
    }
}