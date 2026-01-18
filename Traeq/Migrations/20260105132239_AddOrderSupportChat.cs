using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Traeq.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderSupportChat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderSupportThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderSupportThreads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSupportThreads_AspNetUsers_AdminId",
                        column: x => x.AdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderSupportThreads_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderSupportThreads_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderSupportMessages",
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
                    table.PrimaryKey("PK_OrderSupportMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderSupportMessages_OrderSupportThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "OrderSupportThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupportMessages_ThreadId",
                table: "OrderSupportMessages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupportThreads_AdminId",
                table: "OrderSupportThreads",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupportThreads_OrderId",
                table: "OrderSupportThreads",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSupportThreads_UserId",
                table: "OrderSupportThreads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderSupportMessages");

            migrationBuilder.DropTable(
                name: "OrderSupportThreads");
        }
    }
}
