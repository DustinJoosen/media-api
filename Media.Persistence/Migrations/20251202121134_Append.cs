using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Media.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Append : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "AuthTokens",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "AuthTokens",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AuthTokens",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AuthTokens_Name",
                table: "AuthTokens",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AuthTokens_Name",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "AuthTokens");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AuthTokens");

            migrationBuilder.AlterColumn<Guid>(
                name: "Token",
                table: "AuthTokens",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
