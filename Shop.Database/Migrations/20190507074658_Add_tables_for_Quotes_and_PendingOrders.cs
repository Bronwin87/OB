using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class Add_tables_for_Quotes_and_PendingOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendingOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: false),
                    OrderNumber = table.Column<string>(nullable: true),
                    AccountId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    QuoteId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingOrders_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PendingOrders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    AccountId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotes_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PendingOrderProducts",
                columns: table => new
                {
                    ProductId = table.Column<string>(nullable: false),
                    PendingOrderId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingOrderProducts", x => new { x.PendingOrderId, x.ProductId });
                    table.UniqueConstraint("AK_PendingOrderProducts_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingOrderProducts_PendingOrders_PendingOrderId",
                        column: x => x.PendingOrderId,
                        principalTable: "PendingOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PendingOrderProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuoteProducts",
                columns: table => new
                {
                    ProductId = table.Column<string>(nullable: false),
                    QuoteId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false),
                    Qty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteProducts", x => new { x.QuoteId, x.ProductId });
                    table.UniqueConstraint("AK_QuoteProducts_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuoteProducts_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PendingOrderProducts_ProductId",
                table: "PendingOrderProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingOrders_AccountId",
                table: "PendingOrders",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingOrders_UserId",
                table: "PendingOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteProducts_ProductId",
                table: "QuoteProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_AccountId",
                table: "Quotes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_UserId",
                table: "Quotes",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendingOrderProducts");

            migrationBuilder.DropTable(
                name: "QuoteProducts");

            migrationBuilder.DropTable(
                name: "PendingOrders");

            migrationBuilder.DropTable(
                name: "Quotes");
        }
    }
}
