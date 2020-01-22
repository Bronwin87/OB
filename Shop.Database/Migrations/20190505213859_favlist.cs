using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class favlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouriteLists",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouriteLists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FavouriteListProducts",
                columns: table => new
                {
                    FavouriteListId = table.Column<int>(nullable: false),
                    ProductId = table.Column<string>(nullable: false),
                    Qty = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouriteListProducts", x => new { x.FavouriteListId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_FavouriteListProducts_FavouriteLists_FavouriteListId",
                        column: x => x.FavouriteListId,
                        principalTable: "FavouriteLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavouriteListProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteListProducts_ProductId",
                table: "FavouriteListProducts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouriteLists_UserId",
                table: "FavouriteLists",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouriteListProducts");

            migrationBuilder.DropTable(
                name: "FavouriteLists");
        }
    }
}
