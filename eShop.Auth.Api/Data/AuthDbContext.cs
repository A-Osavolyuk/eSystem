using eShop.Auth.Api.Enums;
using OpenTelemetry.Resources;

namespace eShop.Auth.Api.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<PersonalDataEntity> PersonalData { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<UserPermissionsEntity> UserPermissions { get; set; }
    public DbSet<SecurityTokenEntity> SecurityTokens { get; set; }
    public DbSet<VerificationCodeEntity> Codes { get; set; }
    public DbSet<ProviderEntity> Providers { get; set; }
    public DbSet<LoginTokenEntity> LoginTokens { get; set; }
    public DbSet<UserSecretEntity> UserSecret { get; set; }
    public DbSet<UserProviderEntity> UserProvider { get; set; }
    public DbSet<ResourceEntity> Resources { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasOne(p => p.PersonalData)
                .WithOne()
                .HasForeignKey<UserEntity>(p => p.PersonalDataId)
                .IsRequired(false);
        });
        
        builder.Entity<RoleEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        builder.Entity<VerificationCodeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(6);
            
            entity.Property(x => x.Type)
                .HasConversion(value => value.ToString(), x => Enum.Parse<Verification>(x));
        });

        builder.Entity<UserRoleEntity>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.RoleId });
            entity.HasOne(x => x.Role)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.RoleId);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<PersonalDataEntity>(entity => { entity.HasKey(p => p.Id); });

        builder.Entity<PermissionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.HasOne<ResourceEntity>(x => x.Resource)
                .WithMany()
                .HasForeignKey(x => x.ResourceId);

            entity.Property(x => x.Action)
                .HasConversion(value => value.ToString(), x => Enum.Parse<ActionType>(x));
        });

        builder.Entity<UserPermissionsEntity>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, Id = ur.PermissionId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(ur => ur.UserId);

            entity.HasOne(ur => ur.Permission)
                .WithMany()
                .HasForeignKey(ur => ur.PermissionId);
        });

        builder.Entity<SecurityTokenEntity>(entity =>
        {
            entity.HasKey(k => k.Id);

            entity.Property(t => t.Token).HasColumnType("VARCHAR(MAX)");

            entity.HasOne(t => t.UserEntity)
                .WithOne()
                .HasForeignKey<SecurityTokenEntity>(t => t.UserId);
        });

        builder.Entity<ProviderEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        builder.Entity<LoginTokenEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<LoginTokenEntity>(x => x.UserId);

            entity.HasOne(x => x.Provider)
                .WithOne()
                .HasForeignKey<LoginTokenEntity>(x => x.ProviderId);
        });
        
        builder.Entity<UserSecretEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<UserSecretEntity>(x => x.UserId);
        });

        builder.Entity<UserProviderEntity>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.ProviderId });

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId);
        });
        
        builder.Entity<ResourceEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        builder.Entity<RolePermissionEntity>(entity =>
        {
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            
            entity.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId);

            entity.HasOne(x => x.Permission)
                .WithMany()
                .HasForeignKey(x => x.PermissionId);
        });
    }
}