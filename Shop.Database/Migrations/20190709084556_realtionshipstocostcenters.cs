using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class realtionshipstocostcenters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostCenters_Locations_LocationId",
                table: "CostCenters");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "CostCenters",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "User_Auth_Relationships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: true),
                    AuthorizerId = table.Column<string>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Auth_Relationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Auth_Relationships_AspNetUsers_AuthorizerId",
                        column: x => x.AuthorizerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Auth_Relationships_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Auth_Relationships_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Auth_Relationships_AuthorizerId",
                table: "User_Auth_Relationships",
                column: "AuthorizerId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Auth_Relationships_CostCenterId",
                table: "User_Auth_Relationships",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Auth_Relationships_UserId",
                table: "User_Auth_Relationships",
                column: "UserId");

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

            migrationBuilder.DropTable(
                name: "User_Auth_Relationships");

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
