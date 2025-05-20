using eShop.Domain.Requests.API.Comments;

namespace eShop.Comments.Api.Validation;

public class UpdateCommentValidator : Validator<UpdateCommentRequest>
{
    public UpdateCommentValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required");
        
        RuleFor(x => x.CommentText)
            .NotEmpty().WithMessage("Comment text is required")
            .MinimumLength(8).WithMessage("Comment text must be at least 8 characters")
            .MaximumLength(3000).WithMessage("Comment text cannot exceed 3000 characters");
    }
}