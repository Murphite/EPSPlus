using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EPSPlus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Members_Employers_EmployerId",
                        column: x => x.EmployerId,
                        principalTable: "Employers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BenefitEligibilities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EligibleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenefitEligibilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BenefitEligibilities_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Contributions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContributionType = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContributionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contributions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contributions_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContributionId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Contributions_ContributionId",
                        column: x => x.ContributionId,
                        principalTable: "Contributions",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FullName", "IsActive", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "UserType" },
                values: new object[,]
                {
                    { "user1", 0, "068a1f92-0318-4bfd-8de8-8e3d07ed7ec1", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@techcorp.com", true, null, true, false, null, "ADMIN@TECHCORP.COM", "TECHCORP_ADMIN", "AQAAAAIAAYagAAAAEO02aRMzc9uYn51vnI9lrz+mvny8CeFcvWnDaZ7d+O+CF8rWGXCKLQD30ot7vYbbOA==", null, false, "bc8f81e2-1a8d-438f-aec6-79e02b144ace", false, "techcorp_admin", null },
                    { "user2", 0, "17e9a59a-8eeb-4c29-a57d-eeb48710672d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@innovate.com", true, null, true, false, null, "ADMIN@INNOVATE.COM", "INNOVATE_ADMIN", "AQAAAAIAAYagAAAAEOKO/rtPgzDpGM79y1XJiqXpE55io6HT5uYvNMSrPu9xkpa2EWd+eqYiWeRRIiWdfQ==", null, false, "defdc599-ac15-46ee-ace4-3e1553f20981", false, "innovate_admin", null },
                    { "user3", 0, "2cb6fd10-3945-4de1-a668-06d5960dfbe3", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "member1@example.com", true, null, true, false, null, "MEMBER1@EXAMPLE.COM", "MEMBER_ONE", "AQAAAAIAAYagAAAAEHh2fPzSIqb5CfL+gEfTG1hWWZaTgfb1Z+K9fSgEY7pNMUrZNOfBqlw+DLULYVON5w==", null, false, "cfe8c973-91af-4db4-9730-f1293d771f2a", false, "member_one", null },
                    { "user4", 0, "2a24975b-1c95-4444-b2c9-766840ef4808", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "member2@example.com", true, null, true, false, null, "MEMBER2@EXAMPLE.COM", "MEMBER_TWO", "AQAAAAIAAYagAAAAELvcJ3bJzrH0xRev3MlG4486gFHF8nzyt9i/H7AvvTdgCvsxfdtxLqVRftpFSn5VvQ==", null, false, "9bf49cb9-b969-481e-b3d0-25b7fb90fca3", false, "member_two", null }
                });

            migrationBuilder.InsertData(
                table: "Employers",
                columns: new[] { "Id", "CompanyName", "CreatedAt", "RegistrationNumber", "UserId" },
                values: new object[,]
                {
                    { "1", "TechCorp", new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6263), "123456789", "user1" },
                    { "2", "Innovate Ltd", new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6304), "987654321", "user2" }
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "Age", "CreatedAt", "DateOfBirth", "EmployerId", "UserId" },
                values: new object[,]
                {
                    { "1", 34, new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6376), new DateTime(1990, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "1", "user3" },
                    { "2", 39, new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6389), new DateTime(1985, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "2", "user4" }
                });

            migrationBuilder.InsertData(
                table: "BenefitEligibilities",
                columns: new[] { "Id", "EligibleDate", "MemberId", "Status" },
                values: new object[,]
                {
                    { "1", new DateTime(2030, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6626), "1", true },
                    { "2", new DateTime(2028, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6645), "2", false }
                });

            migrationBuilder.InsertData(
                table: "Contributions",
                columns: new[] { "Id", "Amount", "ContributionDate", "ContributionType", "MemberId", "Status" },
                values: new object[,]
                {
                    { "1", 100.00m, new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6470), 0, "1", "Completed" },
                    { "2", 200.00m, new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6479), 1, "2", "Pending" }
                });

            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "ContributionId", "CreatedAt", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { "1", "1", new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6568), 0, new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6569) },
                    { "2", "2", new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6579), 2, new DateTime(2025, 3, 26, 14, 10, 26, 598, DateTimeKind.Local).AddTicks(6580) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_UserId",
                table: "Admins",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BenefitEligibilities_MemberId",
                table: "BenefitEligibilities",
                column: "MemberId",
                unique: true,
                filter: "[MemberId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Contributions_MemberId",
                table: "Contributions",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Employers_UserId",
                table: "Employers",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Members_EmployerId",
                table: "Members",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_UserId",
                table: "Members",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_ContributionId",
                table: "Transactions",
                column: "ContributionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BenefitEligibilities");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Contributions");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Employers");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
