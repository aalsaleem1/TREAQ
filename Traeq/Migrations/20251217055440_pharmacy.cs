using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traeq.Migrations
{
    /// <inheritdoc />
    public partial class pharmacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyLegalInfos_AspNetUsers_UserId1",
                table: "PharmacyLegalInfos");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyLegalInfos_UserId1",
                table: "PharmacyLegalInfos");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "PharmacyLegalInfos");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "PharmacyLegalInfos",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyLegalInfos_UserId",
                table: "PharmacyLegalInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyLegalInfos_AspNetUsers_UserId",
                table: "PharmacyLegalInfos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PharmacyLegalInfos_AspNetUsers_UserId",
                table: "PharmacyLegalInfos");

            migrationBuilder.DropIndex(
                name: "IX_PharmacyLegalInfos_UserId",
                table: "PharmacyLegalInfos");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "PharmacyLegalInfos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "PharmacyLegalInfos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PharmacyLegalInfos_UserId1",
                table: "PharmacyLegalInfos",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_PharmacyLegalInfos_AspNetUsers_UserId1",
                table: "PharmacyLegalInfos",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
