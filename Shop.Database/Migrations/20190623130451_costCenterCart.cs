using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class costCenterCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostCenterId",
                table: "Carts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CostCenterId",
                table: "Carts",
                column: "CostCenterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_CostCenters_CostCenterId",
                table: "Carts",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_CostCenters_CostCenterId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CostCenterId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CostCenterId",
                table: "Carts");
        }
    }
}
