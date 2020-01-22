using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Shop.Database.Migrations
{
    public partial class renewcostcenterauths2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
