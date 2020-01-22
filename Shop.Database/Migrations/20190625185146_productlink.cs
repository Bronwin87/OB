using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class productlink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductLinks",
                columns: table => new
                {
                    RootId = table.Column<string>(nullable: false),
                    TargetId = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductLinks", x => new { x.RootId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_ProductLinks_Products_RootId",
                        column: x => x.RootId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductLinks_Products_TargetId",
                        column: x => x.TargetId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductLinks_RootId",
                table: "ProductLinks",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductLinks_TargetId",
                table: "ProductLinks",
                column: "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductLinks");
        }
    }
}
