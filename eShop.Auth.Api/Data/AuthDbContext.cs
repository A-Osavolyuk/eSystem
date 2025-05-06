namespace eShop.Auth.Api.Data;

public sealed class AuthDbContext(
    DbContextOptions<AuthDbContext> options) : IdentityDbContext<
        UserEntity, 
        RoleEntity, 
        Guid, 
        UserClaimEntity, 
        UserRoleEntity, 
        UserLoginEntity, 
        RoleClaimEntity,
        UserTokenEntity>(options)
{
    public DbSet<PersonalDataEntity> PersonalData => Set<PersonalDataEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<UserPermissionsEntity> UserPermissions => Set<UserPermissionsEntity>();
    public DbSet<SecurityTokenEntity> SecurityTokens => Set<SecurityTokenEntity>();
    public DbSet<VerificationCodeEntity> Codes => Set<VerificationCodeEntity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>(entity =>
        {
            entity.HasOne(p => p.PersonalData)
                .WithOne()
                .HasForeignKey<UserEntity>(p => p.PersonalDataId);
        });
        
        builder.Entity<VerificationCodeEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(6);
        });

        builder.Entity<UserRoleEntity>(entity =>
        {
            entity.HasOne(x => x.Role)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.RoleId);
            
            entity.HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<PersonalDataEntity>(x =>
        {
            x.HasKey(p => p.Id);
        });

        builder.Entity<PermissionEntity>(x =>
        {
            x.HasKey(p => p.Id);
        });

        builder.Entity<UserPermissionsEntity>(x =>
        {
            x.HasKey(ur => new { ur.UserId, ur.Id });

            x.HasOne(ur => ur.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(ur => ur.UserId);

            x.HasOne(ur => ur.Permission)
                .WithMany(r => r.Permissions)
                .HasForeignKey(ur => ur.Id);
        });

        builder.Entity<SecurityTokenEntity>(x =>
        {
            x.HasKey(k => k.Id);
            
            x.Property(t => t.Token).HasColumnType("VARCHAR(MAX)");
            
            x.HasOne(t => t.UserEntity)
                .WithOne()
                .HasForeignKey<SecurityTokenEntity>(t => t.UserId);
        });
    }
}