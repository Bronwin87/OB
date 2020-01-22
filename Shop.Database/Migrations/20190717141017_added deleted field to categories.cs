using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class addeddeletedfieldtocategories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_Locations_LocationId",
                table: "CostCenters");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "TertiaryCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SubCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "ValueAddedProduct",
                table: "Products",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "MainCategories",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "CostCenters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_Locations_LocationId",
                table: "CostCenters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_Locations_LocationId",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "TertiaryCategories");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SubCategories");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "MainCategories");

            migrationBuilder.AlterColumn<bool>(
                name: "ValueAddedProduct",
                table: "Products",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "CostCenters",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_Locations_LocationId",
                table: "CostCenters",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
