using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traeq.Migrations
{
    /// <inheritdoc />
    public partial class first11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutUs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreateId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EditId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutUs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_PharmacyLegalInfoId",
                table: "Medicines",
                column: "PharmacyLegalInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicines_PharmacyLegalInfos_PharmacyLegalInfoId",
                table: "Medicines",
                column: "PharmacyLegalInfoId",
                principalTable: "PharmacyLegalInfos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicines_PharmacyLegalInfos_PharmacyLegalInfoId",
                table: "Medicines");

            migrationBuilder.DropTable(
                name: "AboutUs");

            migrationBuilder.DropIndex(
                name: "IX_Medicines_PharmacyLegalInfoId",
                table: "Medicines");
        }
    }
}
