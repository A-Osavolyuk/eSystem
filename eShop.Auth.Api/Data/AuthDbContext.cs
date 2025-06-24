namespace eShop.Auth.Api.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<PersonalDataEntity> PersonalData { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<UserPermissionsEntity> UserPermissions { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<CodeEntity> Codes { get; set; }
    public DbSet<ProviderEntity> Providers { get; set; }
    public DbSet<LoginTokenEntity> LoginTokens { get; set; }
    public DbSet<UserSecretEntity> UserSecret { get; set; }
    public DbSet<UserProviderEntity> UserProvider { get; set; }
    public DbSet<ResourceEntity> Resources { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<LockoutStateEntity> LockoutState { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email).HasMaxLength(64);
            entity.Property(x => x.NormalizedEmail).HasMaxLength(64);
            entity.Property(x => x.UserName).HasMaxLength(64);
            entity.Property(x => x.NormalizedUserName).HasMaxLength(64);
            entity.Property(x => x.PhoneNumber).HasMaxLength(17);
            entity.Property(x => x.PasswordHash).HasMaxLength(1000);
        });
        
        builder.Entity<RoleEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
            entity.Property(x => x.NormalizedName).HasMaxLength(64);
        });

        builder.Entity<CodeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).HasMaxLength(6);
            
            entity.Property(x => x.Type)
                .HasConversion(value => value.ToString(), x => Enum.Parse<CodeType>(x));
            
            entity.Property(x => x.Sender)
                .HasConversion(value => value.ToString(), x => Enum.Parse<SenderType>(x));
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

        builder.Entity<PersonalDataEntity>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(x => x.Gender)
                .HasConversion(value => value.ToString(), x => Enum.Parse<Gender>(x));

            entity.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<PersonalDataEntity>(p => p.UserId);
        });

        builder.Entity<PermissionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Name).HasMaxLength(64);
            
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

        builder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(k => k.Id);

            entity.HasOne(t => t.UserEntity)
                .WithOne()
                .HasForeignKey<RefreshTokenEntity>(t => t.UserId);
            
            entity.Property(x => x.Token).HasColumnType("NVARCHAR(MAX)");
        });

        builder.Entity<ProviderEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
        });

        builder.Entity<LoginTokenEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Token).HasMaxLength(6);

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
            entity.Property(x => x.Secret).HasMaxLength(64);

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
            entity.Property(x => x.Name).HasMaxLength(64);
        });

        builder.Entity<RolePermissionEntity>(entity =>
        {
            entity.HasKey(x => new { x.RoleId, x.PermissionId });
            
            entity.HasOne(x => x.Role)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.RoleId);

            entity.HasOne(x => x.Permission)
                .WithMany()
                .HasForeignKey(x => x.PermissionId);
        });

        builder.Entity<LockoutStateEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<LockoutStateEntity>(x => x.UserId);

            entity.Property(x => x.Description).HasMaxLength(3000);
            entity.Property(x => x.Reason)
                .HasConversion(value => value.ToString(), x => Enum.Parse<LockoutReason>(x));;
        });
    }
}