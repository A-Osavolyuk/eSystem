using eShop.Comments.Api.Entities;

namespace eShop.Comments.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CommentEntity> Comments => Set<CommentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CommentEntity>(x => { x.HasKey(p => p.Id); });
    }
}