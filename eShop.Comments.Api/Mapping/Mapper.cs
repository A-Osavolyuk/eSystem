using eShop.Comments.Api.Entities;
using eShop.Domain.DTOs;

namespace eShop.Comments.Api.Mapping;

public static class Mapper
{
    public static CommentDto ToCommentDto(CommentEntity entity)
    {
        return new()
        {
            UpdateDate = entity.UpdatedAt,
            Id = entity.CommentId,
            UserId = entity.UserId,
            CreateDate = entity.CreatedAt,
            Rating = entity.Rating,
            Text = entity.CommentText,
            Images = entity.Images,
            Username = entity.Username
        };
    }

    public static CommentEntity ToCommentEntity(CreateCommentRequest request)
    {
        return new()
        {
            Username = request.Username,
            Images = request.Images,
            Rating = request.Rating,
            CommentText = request.CommentText,
            CreatedAt = DateTime.Now,
            ProductId = request.ProductId,
            UserId = request.UserId
        };
    }

    public static CommentEntity ToCommentEntity(UpdateCommentRequest request)
    {
        return new()
        {
            Username = request.Username,
            Images = request.Images,
            Rating = request.Rating,
            CommentText = request.CommentText,
            UpdatedAt = DateTime.Now,
            ProductId = request.ProductId,
            CommentId = request.CommentId,
            UserId = request.UserId
        };
    }
}