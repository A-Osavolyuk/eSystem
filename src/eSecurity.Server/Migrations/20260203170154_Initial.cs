using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSecurity.Server.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Certificates",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ProtectedCertificate = table.Column<byte[]>(type: "bytea", nullable: false),
                    ProtectedPassword = table.Column<byte[]>(type: "bytea", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RotateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Audience = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ClientType = table.Column<string>(type: "text", nullable: false),
                    AccessTokenType = table.Column<string>(type: "text", nullable: false),
                    RequireClientSecret = table.Column<bool>(type: "boolean", nullable: false),
                    RequirePkce = table.Column<bool>(type: "boolean", nullable: false),
                    Secret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AllowOfflineAccess = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshTokenRotationEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    RefreshTokenLifetime = table.Column<long>(type: "bigint", nullable: false),
                    AllowFrontChannelLogout = table.Column<bool>(type: "boolean", nullable: false),
                    AllowBackChannelLogout = table.Column<bool>(type: "boolean", nullable: false),
                    SubjectType = table.Column<string>(type: "text", nullable: false),
                    SectorIdentifierUri = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LogoUri = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ClientUri = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceOwners",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceOwners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedUsername = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    UsernameChangeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AccountConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "integer", nullable: false),
                    CodeResendAttempts = table.Column<int>(type: "integer", nullable: false),
                    CodeResendAvailableDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ZoneInfo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Locale = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientAllowedScopes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAllowedScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAllowedScopes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientGrantTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGrantTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGrantTypes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientResponseTypes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientResponseTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientResponseTypes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientTokenAuthMethods",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientTokenAuthMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientTokenAuthMethods_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientUris",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Uri = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_ResourceOwners_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "public",
                        principalTable: "ResourceOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationCodes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Protocol = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nonce = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RedirectUri = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CodeChallenge = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CodeChallengeMethod = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Codes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CodeHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Sender = table.Column<string>(type: "text", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Codes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consents",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consents_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consents_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LockoutStates",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    Permanent = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockoutStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LockoutStates_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PairwiseSubjects",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SectorIdentifier = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SubjectIdentifier = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PairwiseSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PairwiseSubjects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PairwiseSubjects_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Hash = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passwords_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalData",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    MiddleName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Gender = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StreetAddress = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Locality = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Region = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    PostalCode = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    Country = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalData_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    AuthenticationMethods = table.Column<string[]>(type: "text[]", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClients",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClients", x => new { x.UserId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_UserClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClients_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    Browser = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Device = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Os = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Location = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FirstSeen = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSeen = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BlockedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEmails",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEmails_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLinkedAccounts",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinkedAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLinkedAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPhoneNumbers",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    VerifiedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhoneNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPhoneNumbers_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRecoveryCodes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProtectedCode = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecoveryCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRecoveryCodes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "public",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecret",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Secret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecret_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTwoFactorMethods",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Preferred = table.Column<bool>(type: "boolean", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTwoFactorMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTwoFactorMethods_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Verifications",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Verifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalSchema: "public",
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrantedScopes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantedScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrantedScopes_Consents_ConsentId",
                        column: x => x.ConsentId,
                        principalSchema: "public",
                        principalTable: "Consents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientSessions",
                schema: "public",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSessions", x => new { x.ClientId, x.SessionId });
                    table.ForeignKey(
                        name: "FK_ClientSessions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientSessions_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "public",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpaqueTokens",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TokenType = table.Column<string>(type: "text", nullable: false),
                    TokenHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Subject = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Revoked = table.Column<bool>(type: "boolean", nullable: false),
                    RevokedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpaqueTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpaqueTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "public",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpaqueTokens_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalSchema: "public",
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passkeys",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthenticatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CredentialId = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PublicKey = table.Column<byte[]>(type: "bytea", nullable: false),
                    Domain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SignCount = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LastSeenDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passkeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passkeys_UserDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "public",
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                schema: "public",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "public",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "public",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                schema: "public",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => new { x.UserId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "public",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "public",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpaqueTokensScopes",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TokenId = table.Column<Guid>(type: "uuid", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpaqueTokensScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpaqueTokensScopes_OpaqueTokens_TokenId",
                        column: x => x.TokenId,
                        principalSchema: "public",
                        principalTable: "OpaqueTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_ClientId",
                schema: "public",
                table: "AuthorizationCodes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_UserId",
                schema: "public",
                table: "AuthorizationCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAllowedScopes_ClientId",
                schema: "public",
                table: "ClientAllowedScopes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId",
                schema: "public",
                table: "ClientGrantTypes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientResponseTypes_ClientId",
                schema: "public",
                table: "ClientResponseTypes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSessions_SessionId",
                schema: "public",
                table: "ClientSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientTokenAuthMethods_ClientId",
                schema: "public",
                table: "ClientTokenAuthMethods",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUris_ClientId",
                schema: "public",
                table: "ClientUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_UserId",
                schema: "public",
                table: "Codes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_ClientId",
                schema: "public",
                table: "Consents",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UserId",
                schema: "public",
                table: "Consents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantedScopes_ConsentId",
                schema: "public",
                table: "GrantedScopes",
                column: "ConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_LockoutStates_UserId",
                schema: "public",
                table: "LockoutStates",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokens_ClientId",
                schema: "public",
                table: "OpaqueTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokens_SessionId",
                schema: "public",
                table: "OpaqueTokens",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokensScopes_TokenId",
                schema: "public",
                table: "OpaqueTokensScopes",
                column: "TokenId");

            migrationBuilder.CreateIndex(
                name: "IX_PairwiseSubjects_ClientId",
                schema: "public",
                table: "PairwiseSubjects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PairwiseSubjects_UserId",
                schema: "public",
                table: "PairwiseSubjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Passkeys_DeviceId",
                schema: "public",
                table: "Passkeys",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passwords_UserId",
                schema: "public",
                table: "Passwords",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ResourceId",
                schema: "public",
                table: "Permissions",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalData_UserId",
                schema: "public",
                table: "PersonalData",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_OwnerId",
                schema: "public",
                table: "Resources",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                schema: "public",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                schema: "public",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClients_ClientId",
                schema: "public",
                table: "UserClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                schema: "public",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmails_UserId",
                schema: "public",
                table: "UserEmails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinkedAccounts_UserId",
                schema: "public",
                table: "UserLinkedAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                schema: "public",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhoneNumbers_UserId",
                schema: "public",
                table: "UserPhoneNumbers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRecoveryCodes_UserId",
                schema: "public",
                table: "UserRecoveryCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "public",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecret_UserId",
                schema: "public",
                table: "UserSecret",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTwoFactorMethods_UserId",
                schema: "public",
                table: "UserTwoFactorMethods",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_UserId",
                schema: "public",
                table: "Verifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationCodes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Certificates",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientAllowedScopes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientGrantTypes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientResponseTypes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientSessions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientTokenAuthMethods",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ClientUris",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Codes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GrantedScopes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "LockoutStates",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OpaqueTokensScopes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PairwiseSubjects",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Passkeys",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Passwords",
                schema: "public");

            migrationBuilder.DropTable(
                name: "PersonalData",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RolePermissions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserClients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserEmails",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserLinkedAccounts",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserPermissions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserPhoneNumbers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserRecoveryCodes",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserSecret",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserTwoFactorMethods",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Verifications",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Consents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "OpaqueTokens",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserDevices",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Sessions",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Resources",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ResourceOwners",
                schema: "public");
        }
    }
}
