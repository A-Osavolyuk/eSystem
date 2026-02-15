using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<PasswordEntity> Passwords { get; set; }
    public DbSet<UserEmailEntity> UserEmails { get; set; }
    public DbSet<UserPhoneNumberEntity> UserPhoneNumbers { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<UserSecretEntity> UserSecret { get; set; }
    public DbSet<UserTwoFactorMethodEntity> UserTwoFactorMethods { get; set; }
    public DbSet<UserLinkedAccountEntity> UserLinkedAccounts { get; set; }
    public DbSet<UserDeviceEntity> UserDevices { get; set; }
    public DbSet<PasskeyEntity> Passkeys { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<PersonalDataEntity> PersonalData { get; set; }
    public DbSet<CodeEntity> Codes { get; set; }
    public DbSet<UserLockoutStateEntity> LockoutStates { get; set; }
    public DbSet<UserRecoveryCodeEntity> UserRecoveryCodes { get; set; }
    public DbSet<VerificationEntity> Verifications { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<UserClientEntity> UserClients { get; set; }
    public DbSet<ClientAllowedScopeEntity> ClientAllowedScopes { get; set; }
    public DbSet<ClientGrantTypeEntity> ClientGrantTypes { get; set; }
    public DbSet<ClientUriEntity> ClientUris { get; set; }
    public DbSet<ClientTokenAuthMethodEntity> ClientTokenAuthMethods { get; set; }
    public DbSet<ClientResponseTypeEntity> ClientResponseTypes { get; set; }
    public DbSet<ClientAudienceEntity> ClientAudiences { get; set; }
    public DbSet<GrantedScopeEntity> GrantedScopes { get; set; }
    public DbSet<SessionEntity> Sessions { get; set; }
    public DbSet<AuthorizationCodeEntity> AuthorizationCodes { get; set; }
    public DbSet<ConsentEntity> Consents { get; set; }
    public DbSet<SigningCertificateEntity> Certificates { get; set; }
    public DbSet<OpaqueTokenEntity> OpaqueTokens { get; set; }
    public DbSet<OpaqueTokenScopeEntity> OpaqueTokensScopes { get; set; }
    public DbSet<PairwiseSubjectEntity> PairwiseSubjects { get; set; }
    public DbSet<ClientSessionEntity> ClientSessions { get; set; }
    public DbSet<ScopeEntity> Scopes { get; set; }
    public DbSet<GrantTypeEntity> GrantTypes { get; set; }
    public DbSet<ResponseTypeEntity> ResponseTypes { get; set; }
    public DbSet<TokenAuthMethodEntity> TokenAuthMethods { get; set; }
    public DbSet<DeviceCodeEntity> DeviceCodes { get; set; }
    public DbSet<OpaqueTokenAudienceEntity> OpaqueTokenAudiences { get; set; }
    public DbSet<CibaRequestEntity> CibaRequests { get; set; }
    public DbSet<AuthenticationSessionEntity> AuthenticationSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("public");
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}