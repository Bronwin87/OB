using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenterAuth_CostCenters_CostCenterId",
                table: "CostCenterAuth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostCenterAuth",
                table: "CostCenterAuth");

            migrationBuilder.RenameTable(
                name: "CostCenterAuth",
                newName: "CostCenterAuths");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Locations",
                newName: "Name");

           // migrationBuilder.RenameIndex(
           //     name: "IX_CostCenterAuth_CostCenterId",
           //     table: "CostCenterAuths",
           //     newName: "IX_CostCenterAuths_CostCenterId");

            migrationBuilder.AddColumn<int>(
                name: "Delivery",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Disclaimer",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "HasVoucher",
                table: "Orders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalSubtotal",
                table: "Orders",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductCount",
                table: "Orders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Vat",
                table: "Orders",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Delivery",
                table: "Carts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Disclaimer",
                table: "Carts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Discount",
                table: "Carts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "HasVoucher",
                table: "Carts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalSubtotal",
                table: "Carts",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductCount",
                table: "Carts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "SubTotal",
                table: "Carts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Vat",
                table: "Carts",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Carts",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostCenterAuths",
                table: "CostCenterAuths",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LocationAuth",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorizerId = table.Column<string>(nullable: true),
                    Userid = table.Column<string>(nullable: true),
                    LocationId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationAuth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationAuth_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LocationAuth_LocationId",
                table: "LocationAuth",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenterAuths_CostCenters_CostCenterId",
                table: "CostCenterAuths",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenterAuths_CostCenters_CostCenterId",
                table: "CostCenterAuths");

            migrationBuilder.DropTable(
                name: "LocationAuth");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostCenterAuths",
                table: "CostCenterAuths");

            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Disclaimer",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "HasVoucher",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OriginalSubtotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductCount",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Delivery",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Disclaimer",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "HasVoucher",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "OriginalSubtotal",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductCount",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Carts");

            migrationBuilder.RenameTable(
                name: "CostCenterAuths",
                newName: "CostCenterAuth");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Locations",
                newName: "CompanyName");

          //  migrationBuilder.RenameIndex(
          //      name: "IX_CostCenterAuths_CostCenterId",
          //      table: "CostCenterAuth",
          //      newName: "IX_CostCenterAuth_CostCenterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostCenterAuth",
                table: "CostCenterAuth",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CostCenterAuth_CostCenters_CostCenterId",
                table: "CostCenterAuth",
                column: "CostCenterId",
                principalTable: "CostCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
