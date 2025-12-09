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
            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProtectedCertificate = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ProtectedPassword = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    RotateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Audience = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ClientType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccessTokenType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequireClientSecret = table.Column<bool>(type: "bit", nullable: false),
                    RequirePkce = table.Column<bool>(type: "bit", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AllowOfflineAccess = table.Column<bool>(type: "bit", nullable: false),
                    RefreshTokenRotationEnabled = table.Column<bool>(type: "bit", nullable: false),
                    RefreshTokenLifetime = table.Column<long>(type: "bigint", nullable: false),
                    SubjectType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectorIdentifierUri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LogoUri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClientUri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceOwners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceOwners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scopes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NormalizedUsername = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    UsernameChangeDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    AccountConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    CodeResendAttempts = table.Column<int>(type: "int", nullable: false),
                    CodeResendAvailableDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ZoneInfo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClientGrantTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientGrantTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientGrantTypes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientUris",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ClientEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUris", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientUris_Clients_ClientEntityId",
                        column: x => x.ClientEntityId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientUris_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resources_ResourceOwners_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "ResourceOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientAllowedScopes",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAllowedScopes", x => new { x.ClientId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_ClientAllowedScopes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientAllowedScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Codes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consents_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LockoutStates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Permanent = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LockoutStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LockoutStates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PairwiseSubjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorIdentifier = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubjectIdentifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PairwiseSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PairwiseSubjects_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PairwiseSubjects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Hash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passwords_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Locality = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    Region = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalData_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClients",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClients", x => new { x.UserId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_UserClients_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsTrusted = table.Column<bool>(type: "bit", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Device = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Os = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    FirstSeen = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    LastSeen = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    BlockedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEmails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLinkedAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinkedAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLinkedAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerifiedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPhoneNumbers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPhoneNumbers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRecoveryCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProtectedCode = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRecoveryCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRecoveryCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSecret",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecret", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecret_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTwoFactorMethods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Preferred = table.Column<bool>(type: "bit", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTwoFactorMethods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTwoFactorMethods_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Verifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Verifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Verifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrantedScopes",
                columns: table => new
                {
                    ConsentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantedScopes", x => new { x.ScopeId, x.ConsentId });
                    table.ForeignKey(
                        name: "FK_GrantedScopes_Consents_ConsentId",
                        column: x => x.ConsentId,
                        principalTable: "Consents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrantedScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nonce = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RedirectUri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CodeChallenge = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodeChallengeMethod = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Used = table.Column<bool>(type: "bit", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_UserDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Passkeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthenticatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CredentialId = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Domain = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SignCount = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    LastSeenDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passkeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Passkeys_UserDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpireDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_UserDevices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "UserDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OAuthSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LinkedAccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Token = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SignType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OAuthSessions_UserLinkedAccounts_LinkedAccountId",
                        column: x => x.LinkedAccountId,
                        principalTable: "UserLinkedAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => new { x.UserId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpaqueTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Revoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpaqueTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpaqueTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpaqueTokens_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpaqueTokensScopes",
                columns: table => new
                {
                    TokenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScopeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UpdateDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpaqueTokensScopes", x => new { x.TokenId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_OpaqueTokensScopes_OpaqueTokens_TokenId",
                        column: x => x.TokenId,
                        principalTable: "OpaqueTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpaqueTokensScopes_Scopes_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_ClientId",
                table: "AuthorizationCodes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_DeviceId",
                table: "AuthorizationCodes",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAllowedScopes_ScopeId",
                table: "ClientAllowedScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientGrantTypes_ClientId",
                table: "ClientGrantTypes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUris_ClientEntityId",
                table: "ClientUris",
                column: "ClientEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUris_ClientId",
                table: "ClientUris",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Codes_UserId",
                table: "Codes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_ClientId",
                table: "Consents",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Consents_UserId",
                table: "Consents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantedScopes_ConsentId",
                table: "GrantedScopes",
                column: "ConsentId");

            migrationBuilder.CreateIndex(
                name: "IX_LockoutStates_UserId",
                table: "LockoutStates",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OAuthSessions_LinkedAccountId",
                table: "OAuthSessions",
                column: "LinkedAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokens_ClientId",
                table: "OpaqueTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokens_SessionId",
                table: "OpaqueTokens",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_OpaqueTokensScopes_ScopeId",
                table: "OpaqueTokensScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_PairwiseSubjects_ClientId",
                table: "PairwiseSubjects",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_PairwiseSubjects_UserId",
                table: "PairwiseSubjects",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Passkeys_DeviceId",
                table: "Passkeys",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passwords_UserId",
                table: "Passwords",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ResourceId",
                table: "Permissions",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalData_UserId",
                table: "PersonalData",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resources_OwnerId",
                table: "Resources",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_DeviceId",
                table: "Sessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClients_ClientId",
                table: "UserClients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEmails_UserId",
                table: "UserEmails",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLinkedAccounts_UserId",
                table: "UserLinkedAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPhoneNumbers_UserId",
                table: "UserPhoneNumbers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRecoveryCodes_UserId",
                table: "UserRecoveryCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecret_UserId",
                table: "UserSecret",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTwoFactorMethods_UserId",
                table: "UserTwoFactorMethods",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Verifications_UserId",
                table: "Verifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizationCodes");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "ClientAllowedScopes");

            migrationBuilder.DropTable(
                name: "ClientGrantTypes");

            migrationBuilder.DropTable(
                name: "ClientUris");

            migrationBuilder.DropTable(
                name: "Codes");

            migrationBuilder.DropTable(
                name: "GrantedScopes");

            migrationBuilder.DropTable(
                name: "LockoutStates");

            migrationBuilder.DropTable(
                name: "OAuthSessions");

            migrationBuilder.DropTable(
                name: "OpaqueTokensScopes");

            migrationBuilder.DropTable(
                name: "PairwiseSubjects");

            migrationBuilder.DropTable(
                name: "Passkeys");

            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "PersonalData");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserClients");

            migrationBuilder.DropTable(
                name: "UserEmails");

            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "UserPhoneNumbers");

            migrationBuilder.DropTable(
                name: "UserRecoveryCodes");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSecret");

            migrationBuilder.DropTable(
                name: "UserTwoFactorMethods");

            migrationBuilder.DropTable(
                name: "Verifications");

            migrationBuilder.DropTable(
                name: "Consents");

            migrationBuilder.DropTable(
                name: "UserLinkedAccounts");

            migrationBuilder.DropTable(
                name: "OpaqueTokens");

            migrationBuilder.DropTable(
                name: "Scopes");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "ResourceOwners");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
