using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class costcenterauth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_Auth_Relationships");

            migrationBuilder.CreateTable(
                name: "CostCenterAuth",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorizerId = table.Column<string>(nullable: true),
                    Userid = table.Column<string>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenterAuth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostCenterAuth_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterAuth_CostCenterId",
                table: "CostCenterAuth",
                column: "CostCenterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostCenterAuth");

            migrationBuilder.CreateTable(
                name: "User_Auth_Relationships",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorizerId = table.Column<string>(nullable: true),
                    CostCenterId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
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
        }
    }
}
