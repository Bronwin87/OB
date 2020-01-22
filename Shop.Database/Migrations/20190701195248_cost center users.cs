using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class costcenterusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorizerId",
                table: "CostCenters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "CostCenters",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_AuthorizerId",
                table: "CostCenters",
                column: "AuthorizerId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_UserId",
                table: "CostCenters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_AspNetUsers_AuthorizerId",
                table: "CostCenters",
                column: "AuthorizerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenters_AspNetUsers_UserId",
                table: "CostCenters",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_AspNetUsers_AuthorizerId",
                table: "CostCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_AspNetUsers_UserId",
                table: "CostCenters");

            migrationBuilder.DropIndex(
                name: "IX_CostCenters_AuthorizerId",
                table: "CostCenters");

            migrationBuilder.DropIndex(
                name: "IX_CostCenters_UserId",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "AuthorizerId",
                table: "CostCenters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CostCenters");
        }
    }
}
