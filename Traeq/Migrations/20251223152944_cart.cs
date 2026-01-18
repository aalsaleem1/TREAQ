using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traeq.Migrations
{
    /// <inheritdoc />
    public partial class cart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MedecineId",
                table: "Carts",
                newName: "MedicineId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_MedicineId",
                table: "Carts",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_PharmacyId",
                table: "Carts",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Medicines_MedicineId",
                table: "Carts",
                column: "MedicineId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_PharmacyLegalInfos_PharmacyId",
                table: "Carts",
                column: "PharmacyId",
                principalTable: "PharmacyLegalInfos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_AspNetUsers_UserId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Medicines_MedicineId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_PharmacyLegalInfos_PharmacyId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_MedicineId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_PharmacyId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "MedicineId",
                table: "Carts",
                newName: "MedecineId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Carts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
