using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockNestMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSessionForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_RefreshToken",
                table: "UserSessions",
                column: "RefreshToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSessions_RefreshToken",
                table: "UserSessions");
        }
    }
}
