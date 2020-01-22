using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class shop_dev2update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenterAuths_CostCenters_CostCenterId",
                table: "CostCenterAuths");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostCenterAuths",
                table: "CostCenterAuths");

            migrationBuilder.RenameTable(
                name: "CostCenterAuths",
                newName: "CostCenterAuth");

            migrationBuilder.RenameIndex(
                name: "IX_CostCenterAuths_CostCenterId",
                table: "CostCenterAuth",
                newName: "IX_CostCenterAuth_CostCenterId");

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "CostCenterAuth",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostCenterAuth",
                table: "CostCenterAuth",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterAuth_Userid",
                table: "CostCenterAuth",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenterAuth_CostCenters_CostCenterId",
                table: "CostCenterAuth",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenterAuth_AspNetUsers_Userid",
                table: "CostCenterAuth",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenterAuth_CostCenters_CostCenterId",
                table: "CostCenterAuth");

            migrationBuilder.DropForeignKey(
                name: "FK_CostCenterAuth_AspNetUsers_Userid",
                table: "CostCenterAuth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostCenterAuth",
                table: "CostCenterAuth");

            migrationBuilder.DropIndex(
                name: "IX_CostCenterAuth_Userid",
                table: "CostCenterAuth");

            migrationBuilder.RenameTable(
                name: "CostCenterAuth",
                newName: "CostCenterAuths");

            migrationBuilder.RenameIndex(
                name: "IX_CostCenterAuth_CostCenterId",
                table: "CostCenterAuths",
                newName: "IX_CostCenterAuths_CostCenterId");

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "CostCenterAuths",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostCenterAuths",
                table: "CostCenterAuths",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenterAuths_CostCenters_CostCenterId",
                table: "CostCenterAuths",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
