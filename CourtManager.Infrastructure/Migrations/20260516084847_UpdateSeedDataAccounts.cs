using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CourtManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "Email", "FirstName", "LastName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { "admin1@court.com", "System", "Admin1", "ADMIN1@COURT.COM", "ADMIN1@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000001", "admin1@court.com" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "Email", "FirstName", "LastName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { "admin2@court.com", "System", "Admin2", "ADMIN2@COURT.COM", "ADMIN2@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000002", "admin2@court.com" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "FirstName", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RefreshToken", "RefreshTokenExpiryTime", "SecurityStamp", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000003"), 0, "20000000-0000-0000-0000-000000000003", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager1@court.com", false, "Court", true, "Manager1", false, null, "MANAGER1@COURT.COM", "MANAGER1@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000003", false, null, null, "20000000-0000-0000-0000-000000000003", false, null, "manager1@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), 0, "20000000-0000-0000-0000-000000000004", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager2@court.com", false, "Court", true, "Manager2", false, null, "MANAGER2@COURT.COM", "MANAGER2@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000004", false, null, null, "20000000-0000-0000-0000-000000000004", false, null, "manager2@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), 0, "20000000-0000-0000-0000-000000000005", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "manager3@court.com", false, "Court", true, "Manager3", false, null, "MANAGER3@COURT.COM", "MANAGER3@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000005", false, null, null, "20000000-0000-0000-0000-000000000005", false, null, "manager3@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), 0, "20000000-0000-0000-0000-000000000006", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "player1@court.com", false, "Pro", true, "Player1", false, null, "PLAYER1@COURT.COM", "PLAYER1@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000006", false, null, null, "20000000-0000-0000-0000-000000000006", false, null, "player1@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000007"), 0, "20000000-0000-0000-0000-000000000007", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "player2@court.com", false, "Pro", true, "Player2", false, null, "PLAYER2@COURT.COM", "PLAYER2@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000007", false, null, null, "20000000-0000-0000-0000-000000000007", false, null, "player2@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000008"), 0, "20000000-0000-0000-0000-000000000008", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "player3@court.com", false, "Casual", true, "Player3", false, null, "PLAYER3@COURT.COM", "PLAYER3@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000008", false, null, null, "20000000-0000-0000-0000-000000000008", false, null, "player3@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000009"), 0, "20000000-0000-0000-0000-000000000009", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "player4@court.com", false, "Casual", true, "Player4", false, null, "PLAYER4@COURT.COM", "PLAYER4@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000009", false, null, null, "20000000-0000-0000-0000-000000000009", false, null, "player4@court.com" },
                    { new Guid("20000000-0000-0000-0000-000000000010"), 0, "20000000-0000-0000-0000-000000000010", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "player5@court.com", false, "Newbie", true, "Player5", false, null, "PLAYER5@COURT.COM", "PLAYER5@COURT.COM", "AQAAAAIAAYagAAAAEMhNOhWJhrehCy84iiKMjD+gAwmKtd2V+CHm4EhzxmaTyXKW9OS5bmKjoFGKqWDFAg==", "0900000010", false, null, null, "20000000-0000-0000-0000-000000000010", false, null, "player5@court.com" }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "AssignedAt" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000003"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000004"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000005"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000006"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000007"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000008"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000009"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000010"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000001"), new Guid("20000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000002"), new Guid("20000000-0000-0000-0000-000000000005") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000006") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000007") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000008") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000009") });

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { new Guid("10000000-0000-0000-0000-000000000003"), new Guid("20000000-0000-0000-0000-000000000010") });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000010"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                columns: new[] { "Email", "FirstName", "LastName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { "john.doe@example.com", "John", "Doe", "JOHN.DOE@EXAMPLE.COM", "JOHN.DOE@EXAMPLE.COM", "", "+1234567890", "john.doe@example.com" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"),
                columns: new[] { "Email", "FirstName", "LastName", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { "jane.smith@example.com", "Jane", "Smith", "JANE.SMITH@EXAMPLE.COM", "JANE.SMITH@EXAMPLE.COM", "", "+0987654321", "jane.smith@example.com" });
        }
    }
}
