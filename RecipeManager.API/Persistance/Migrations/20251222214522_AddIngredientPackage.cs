using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientPackage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PackageSize = table.Column<int>(type: "int", nullable: false),
                    PackageSizeUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientPackage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientPackage_Ingredient_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientPackage_Unit_PackageSizeUnitId",
                        column: x => x.PackageSizeUnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IngredientPackage_Unit_PackageUnitId",
                        column: x => x.PackageUnitId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientPackage_IngredientId",
                table: "IngredientPackage",
                column: "IngredientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientPackage_PackageSizeUnitId",
                table: "IngredientPackage",
                column: "PackageSizeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientPackage_PackageUnitId",
                table: "IngredientPackage",
                column: "PackageUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientPackage");
        }
    }
}
