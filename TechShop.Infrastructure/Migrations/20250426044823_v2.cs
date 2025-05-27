using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Revoked",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "Expires",
                table: "RefreshTokens",
                newName: "ExpiredAt");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "RefreshTokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "RefreshTokens",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "RefreshTokens");

            migrationBuilder.RenameColumn(
                name: "ExpiredAt",
                table: "RefreshTokens",
                newName: "Expires");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Revoked",
                table: "RefreshTokens",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
