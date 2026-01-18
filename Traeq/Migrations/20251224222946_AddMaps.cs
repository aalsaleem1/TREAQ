using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traeq.Migrations
{
    /// <inheritdoc />
    public partial class AddMaps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "UserAddresses",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "UserAddresses",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "PharmacyLegalInfos",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "PharmacyLegalInfos",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "PharmacyLegalInfos");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "PharmacyLegalInfos");
        }
    }
}
