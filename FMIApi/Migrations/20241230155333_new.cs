using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FMIApi.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarageCar");

            migrationBuilder.CreateTable(
                name: "CarGarage",
                columns: table => new
                {
                    CarsId = table.Column<long>(type: "bigint", nullable: false),
                    GaragesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarGarage", x => new { x.CarsId, x.GaragesId });
                    table.ForeignKey(
                        name: "FK_CarGarage_Cars_CarsId",
                        column: x => x.CarsId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarGarage_Garages_GaragesId",
                        column: x => x.GaragesId,
                        principalTable: "Garages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarGarage_GaragesId",
                table: "CarGarage",
                column: "GaragesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarGarage");

            migrationBuilder.CreateTable(
                name: "GarageCar",
                columns: table => new
                {
                    CarId = table.Column<long>(type: "bigint", nullable: false),
                    GarageId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarageCar", x => new { x.CarId, x.GarageId });
                    table.ForeignKey(
                        name: "FK_GarageCar_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GarageCar_Garages_GarageId",
                        column: x => x.GarageId,
                        principalTable: "Garages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarageCar_GarageId",
                table: "GarageCar",
                column: "GarageId");
        }
    }
}
