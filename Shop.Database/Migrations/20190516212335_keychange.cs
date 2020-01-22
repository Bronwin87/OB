using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class keychange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_QuoteProducts_Id",
                table: "QuoteProducts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "QuoteProducts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "QuoteProducts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_QuoteProducts_Id",
                table: "QuoteProducts",
                column: "Id");
        }
    }
}
