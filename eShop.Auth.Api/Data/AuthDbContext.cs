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
    public DbSet<LoginCodeEntity> LoginCodes { get; set; }
    public DbSet<UserSecretEntity> UserSecret { get; set; }
    public DbSet<UserProviderEntity> UserProvider { get; set; }
    public DbSet<ResourceEntity> Resources { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<LockoutStateEntity> LockoutStates { get; set; }
    public DbSet<LockoutReasonEntity> LockoutReasons { get; set; }
    public DbSet<ResourceOwnerEntity> ResourceOwners { get; set; }
    public DbSet<RecoveryCodeEntity> RecoveryCodes { get; set; }
    public DbSet<UserChangesEntity> UserChanges { get; set; }
    public DbSet<OAuthProviderEntity> OAuthProviders { get; set; }
    public DbSet<OAuthSessionEntity> OAuthSessions { get; set; }
    public DbSet<UserLinkedAccountEntity> UserOAuthProviders { get; set; }
    public DbSet<LoginSessionEntity> LoginSessions { get; set; }
    public DbSet<UserDeviceEntity> UserDevices { get; set; }
    public DbSet<UserPasskeyEntity> UserPasskeys { get; set; }
    public DbSet<VerificationEntity> Verifications { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(64);
            entity.Property(x => x.NormalizedEmail).HasMaxLength(64);
            entity.Property(x => x.RecoveryEmail).HasMaxLength(64);
            entity.Property(x => x.NormalizedRecoveryEmail).HasMaxLength(64);
            entity.Property(x => x.Username).HasMaxLength(64);
            entity.Property(x => x.NormalizedUsername).HasMaxLength(64);
            entity.Property(x => x.PhoneNumber).HasMaxLength(18);
            entity.Property(x => x.PasswordHash).HasMaxLength(1000);
            
            entity.HasOne(p => p.PersonalData)
                .WithOne(u => u.User)
                .HasForeignKey<UserEntity>(p => p.PersonalDataId);
        });
        
        builder.Entity<UserChangesEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Value).HasMaxLength(500);
            entity.Property(x => x.Field).HasEnumConversion();

            entity.HasOne(x => x.User)
                .WithMany(x => x.Changes)
                .HasForeignKey(x => x.UserId);
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
            entity.Property(x => x.CodeHash).HasMaxLength(200);
            entity.Property(x => x.Type).HasEnumConversion();
            entity.Property(x => x.Sender).HasEnumConversion();
            entity.Property(x => x.Resource).HasEnumConversion();
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
            entity.Property(x => x.Gender).HasEnumConversion();
            entity.Property(x => x.FirstName).HasMaxLength(64);
            entity.Property(x => x.LastName).HasMaxLength(64);
            entity.Property(x => x.MiddleName).HasMaxLength(64);
        });

        builder.Entity<PermissionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
            
            entity.HasOne<ResourceEntity>(x => x.Resource)
                .WithMany()
                .HasForeignKey(x => x.ResourceId);

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
            entity.Property(x => x.Token).HasMaxLength(3000);

            entity.HasOne(t => t.UserEntity)
                .WithOne()
                .HasForeignKey<RefreshTokenEntity>(t => t.UserId);
        });

        builder.Entity<ProviderEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
        });

        builder.Entity<LoginCodeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CodeHash).HasMaxLength(200);

            entity.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<LoginCodeEntity>(x => x.UserId);

            entity.HasOne(x => x.Provider)
                .WithOne()
                .HasForeignKey<LoginCodeEntity>(x => x.ProviderId);
        });
        
        builder.Entity<UserSecretEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Secret).HasMaxLength(200);

            entity.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<UserSecretEntity>(x => x.UserId);
        });

        builder.Entity<UserProviderEntity>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.ProviderId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.Providers)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId);
        });
        
        builder.Entity<ResourceEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);

            entity.HasOne(x => x.Owner)
                .WithMany()
                .HasForeignKey(x => x.OwnerId);
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
            entity.Property(x => x.Description).HasMaxLength(3000);
            
            entity.HasOne(p => p.User)
                .WithOne(u => u.LockoutState)
                .HasForeignKey<LockoutStateEntity>(x => x.UserId);
            
            entity.HasOne(p => p.Reason)
                .WithMany()
                .HasForeignKey(x => x.ReasonId)
                .IsRequired(false);
        });

        builder.Entity<LockoutReasonEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Description).HasMaxLength(3000);
            entity.Property(x => x.Name).HasMaxLength(64);
            entity.Property(x => x.Code).HasMaxLength(64);
            entity.Property(x => x.Type).HasEnumConversion();
            entity.Property(x => x.Period).HasEnumConversion();
        });

        builder.Entity<ResourceOwnerEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
        });

        builder.Entity<RecoveryCodeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CodeHash).HasMaxLength(200);
            
            entity.HasOne(x => x.User)
                .WithMany(x => x.RecoveryCodes)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<OAuthProviderEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
        });

        builder.Entity<UserLinkedAccountEntity>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.ProviderId });
            
            entity.HasOne(x => x.User)
                .WithMany(x => x.LinkedAccounts)
                .HasForeignKey(x => x.UserId);
            
            entity.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId);
        });

        builder.Entity<OAuthSessionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Token).HasMaxLength(32);
            entity.Property(x => x.SignType).HasEnumConversion();
            
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired(false);
            
            entity.HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .IsRequired(false);
        });
        
        builder.Entity<UserDeviceEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Device).HasMaxLength(64);
            entity.Property(x => x.Browser).HasMaxLength(64);
            entity.Property(x => x.OS).HasMaxLength(64);
            entity.Property(x => x.UserAgent).HasMaxLength(128);
            entity.Property(x => x.Location).HasMaxLength(128);
            entity.Property(x => x.IpAddress).HasMaxLength(15);
            
            entity.HasOne(x => x.User)
                .WithMany(x => x.Devices)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<LoginSessionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.Provider).HasMaxLength(64);
            entity.Property(x => x.UserAgent).HasMaxLength(128);
            entity.Property(x => x.IpAddress).HasMaxLength(15);
            entity.Property(x => x.Type).HasEnumConversion();
            entity.Property(x => x.Status).HasEnumConversion();

            entity.HasOne(x => x.Device)
                .WithMany()
                .HasForeignKey(x => x.DeviceId);
        });

        builder.Entity<UserPasskeyEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            
            entity.Property(x => x.CredentialId).HasMaxLength(1000);
            entity.Property(x => x.Domain).HasMaxLength(100);
            entity.Property(x => x.DisplayName).HasMaxLength(100);
            entity.Property(x => x.Type).HasMaxLength(32);
            
            entity.HasOne(x => x.User)
                .WithMany(x => x.Passkeys)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<VerificationEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Resource).HasEnumConversion();
            entity.Property(x => x.Type).HasEnumConversion();
            
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        });
    }
}