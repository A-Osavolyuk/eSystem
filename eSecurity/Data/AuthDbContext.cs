using eSecurity.Data.Entities;
using eSystem.Core.Data;

namespace eSecurity.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PasswordEntity> Passwords { get; set; }
    public DbSet<UserEmailEntity> UserEmails { get; set; }
    public DbSet<UserPhoneNumberEntity> UserPhoneNumbers { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<UserPermissionsEntity> UserPermissions { get; set; }
    public DbSet<UserSecretEntity> UserSecret { get; set; }
    public DbSet<UserTwoFactorMethodEntity> UserTwoFactorMethods { get; set; }
    public DbSet<UserLinkedAccountEntity> UserLinkedAccounts { get; set; }
    public DbSet<UserDeviceEntity> UserDevices { get; set; }
    public DbSet<PasskeyEntity> Passkeys { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PersonalDataEntity> PersonalData { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<CodeEntity> Codes { get; set; }
    public DbSet<ResourceEntity> Resources { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<UserLockoutStateEntity> LockoutStates { get; set; }
    public DbSet<ResourceOwnerEntity> ResourceOwners { get; set; }
    public DbSet<UserRecoveryCodeEntity> UserRecoveryCodes { get; set; }
    public DbSet<OAuthSessionEntity> OAuthSessions { get; set; }
    public DbSet<VerificationEntity> Verifications { get; set; }
    public DbSet<UserVerificationMethodEntity> UserVerificationMethods { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<UserClientEntity> UserClients { get; set; }
    public DbSet<ClientAllowedScopeEntity> ClientAllowedScopes { get; set; }
    public DbSet<ClientRedirectUriEntity> ClientRedirectUris { get; set; }
    public DbSet<ClientPostLogoutRedirectUriEntity> ClientPostLogoutUris { get; set; }
    public DbSet<ClientGrantTypeEntity> ClientGrantTypes { get; set; }
    public DbSet<ScopeEntity> Scopes { get; set; }
    public DbSet<GrantedScopeEntity> GrantedScopes { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<AuthorizationCodeEntity> AuthorizationCodes { get; set; }
    public DbSet<ConsentEntity> Consents { get; set; }
    public DbSet<SigningKeyEntity> SigningKeys { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Username).HasMaxLength(64);
            entity.Property(x => x.NormalizedUsername).HasMaxLength(64);

            entity.HasOne(p => p.PersonalData)
                .WithOne(u => u.User)
                .HasForeignKey<UserEntity>(p => p.PersonalDataId);
        });

        builder.Entity<PasswordEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Hash).HasMaxLength(1000);

            entity.HasOne(x => x.User)
                .WithOne(x => x.Password)
                .HasForeignKey<PasswordEntity>(x => x.UserId);
        });

        builder.Entity<UserEmailEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email).HasMaxLength(64);
            entity.Property(x => x.NormalizedEmail).HasMaxLength(64);
            entity.Property(x => x.Type).HasEnumConversion();

            entity.HasOne(u => u.User)
                .WithMany(x => x.Emails)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<UserPhoneNumberEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.PhoneNumber).HasMaxLength(18);
            entity.Property(x => x.Type).HasEnumConversion();

            entity.HasOne(u => u.User)
                .WithMany(x => x.PhoneNumbers)
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
            entity.Property(x => x.Action).HasEnumConversion();
            entity.Property(x => x.Sender).HasEnumConversion();
            entity.Property(x => x.Purpose).HasEnumConversion();
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

        builder.Entity<UserSecretEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Secret).HasMaxLength(200);

            entity.HasOne(x => x.User)
                .WithOne(x => x.Secret)
                .HasForeignKey<UserSecretEntity>(x => x.UserId);
        });

        builder.Entity<UserTwoFactorMethodEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Method).HasEnumConversion();

            entity.HasOne(x => x.User)
                .WithMany(x => x.TwoFactorMethods)
                .HasForeignKey(x => x.UserId);
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

        builder.Entity<UserLockoutStateEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Description).HasMaxLength(3000);

            entity.HasOne(p => p.User)
                .WithOne(u => u.LockoutState)
                .HasForeignKey<UserLockoutStateEntity>(x => x.UserId);
        });

        builder.Entity<ResourceOwnerEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(64);
        });

        builder.Entity<UserRecoveryCodeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProtectedCode).HasMaxLength(150);

            entity.HasOne(x => x.User)
                .WithMany(x => x.RecoveryCodes)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<UserLinkedAccountEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Type).HasEnumConversion();

            entity.HasOne(x => x.User)
                .WithMany(x => x.LinkedAccounts)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<OAuthSessionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Token).HasMaxLength(32);
            entity.Property(x => x.SignType).HasEnumConversion();

            entity.HasOne(x => x.LinkedAccount)
                .WithMany()
                .HasForeignKey(x => x.LinkedAccountId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
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

        builder.Entity<PasskeyEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.CredentialId).HasMaxLength(1000);
            entity.Property(x => x.Domain).HasMaxLength(100);
            entity.Property(x => x.DisplayName).HasMaxLength(100);
            entity.Property(x => x.Type).HasMaxLength(32);

            entity.HasOne(x => x.Device)
                .WithOne(x => x.Passkey)
                .HasForeignKey<PasskeyEntity>(x => x.DeviceId);
        });

        builder.Entity<VerificationEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Purpose).HasEnumConversion();
            entity.Property(x => x.Action).HasEnumConversion();

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<UserVerificationMethodEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(x => x.Method).HasEnumConversion();

            entity.HasOne(x => x.User)
                .WithMany(x => x.VerificationMethods)
                .HasForeignKey(x => x.UserId);
        });

        builder.Entity<ScopeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100);
            entity.Property(x => x.Description).HasMaxLength(1000);
        });

        builder.Entity<ClientAllowedScopeEntity>(entity =>
        {
            entity.HasKey(x => new { x.ClientId, x.ScopeId });

            entity.HasOne(x => x.Client)
                .WithMany(x => x.AllowedScopes)
                .HasForeignKey(x => x.ClientId);

            entity.HasOne(x => x.Scope)
                .WithMany()
                .HasForeignKey(x => x.ScopeId);
        });

        builder.Entity<ClientRedirectUriEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Uri).HasMaxLength(200);

            entity.HasOne(x => x.Client)
                .WithMany(x => x.RedirectUris)
                .HasForeignKey(x => x.ClientId);
        });

        builder.Entity<ClientPostLogoutRedirectUriEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Uri).HasMaxLength(200);

            entity.HasOne(x => x.Client)
                .WithMany(x => x.PostLogoutRedirectUris)
                .HasForeignKey(x => x.ClientId);
        });

        builder.Entity<ClientGrantTypeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Type).HasMaxLength(50);

            entity.HasOne(x => x.Client)
                .WithMany(x => x.GrantTypes)
                .HasForeignKey(x => x.ClientId);
        });

        builder.Entity<ClientEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ClientId).HasMaxLength(200);
            entity.Property(x => x.ClientSecret).HasMaxLength(200);
            entity.Property(x => x.Name).HasMaxLength(100);
            entity.Property(x => x.LogoUri).HasMaxLength(100);
            entity.Property(x => x.ClientUri).HasMaxLength(100);
            entity.Property(x => x.Type).HasEnumConversion();
            entity.Property(x => x.RefreshTokenLifetime).HasConversion(
                time => time.Ticks,
                ticks => TimeSpan.FromTicks(ticks));
        });

        builder.Entity<AuthorizationCodeEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.RedirectUri).HasMaxLength(200);
            entity.Property(x => x.CodeChallenge).HasMaxLength(200);
            entity.Property(x => x.CodeChallengeMethod).HasMaxLength(16);
            entity.Property(x => x.Code).HasMaxLength(20);
            entity.Property(x => x.Nonce).HasMaxLength(20);

            entity.HasOne(x => x.Device)
                .WithMany()
                .HasForeignKey(x => x.DeviceId);

            entity.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId);
        });

        builder.Entity<SessionEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Device)
                .WithMany(x => x.Sessions)
                .HasForeignKey(x => x.DeviceId);
        });

        builder.Entity<ConsentEntity>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany(x => x.Consents)
                .HasForeignKey(x => x.UserId);

            entity.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId);
        });

        builder.Entity<GrantedScopeEntity>(entity =>
        {
            entity.HasKey(x => new { x.ScopeId, x.ConsentId });

            entity.HasOne(x => x.Consent)
                .WithMany(x => x.GrantedScopes)
                .HasForeignKey(x => x.ConsentId);

            entity.HasOne(x => x.Scope)
                .WithMany()
                .HasForeignKey(x => x.ScopeId);
        });

        builder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.HasKey(k => k.Id);
            entity.Property(x => x.Token).HasMaxLength(20);

            entity.HasOne(rt => rt.Session)
                .WithMany(s => s.RefreshTokens)
                .HasForeignKey(rt => rt.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rt => rt.Client)
                .WithMany()
                .HasForeignKey(rt => rt.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserClientEntity>(entity =>
        {
            entity.HasKey(x => new { x.UserId, x.ClientId });

            entity.HasOne(x => x.User)
                .WithMany(x => x.Clients)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SigningKeyEntity>(entity =>
        {
            entity.HasKey(x => x.Id);
        });
    }
}