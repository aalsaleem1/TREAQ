using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traeq.Migrations
{
    public partial class AddPharmacySupportChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PharmacySupportThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PharmacyId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacySupportThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacySupportThreads_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PharmacySupportThreads_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PharmacySupportThreads_PharmacyLegalInfos_PharmacyId",
                        column: x => x.PharmacyId,
                        principalTable: "PharmacyLegalInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PharmacySupportMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThreadId = table.Column<int>(type: "int", nullable: false),
                    SenderType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MessageText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PharmacySupportMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PharmacySupportMessages_PharmacySupportThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "PharmacySupportThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PharmacySupportThreads_UserId",
                table: "PharmacySupportThreads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacySupportThreads_PharmacyId",
                table: "PharmacySupportThreads",
                column: "PharmacyId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacySupportThreads_OrderId",
                table: "PharmacySupportThreads",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PharmacySupportMessages_ThreadId",
                table: "PharmacySupportMessages",
                column: "ThreadId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PharmacySupportMessages");

            migrationBuilder.DropTable(
                name: "PharmacySupportThreads");
        }
    }
}
