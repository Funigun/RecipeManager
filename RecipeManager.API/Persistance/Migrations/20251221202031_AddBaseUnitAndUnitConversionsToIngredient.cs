using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseUnitAndUnitConversionsToIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BaseUnit",
                table: "Ingredient",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IngredientUnitConversion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitToConvertId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ratio = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientUnitConversion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientUnitConversion_Ingredient_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientUnitConversion_Unit_UnitToConvertId",
                        column: x => x.UnitToConvertId,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_BaseUnit",
                table: "Ingredient",
                column: "BaseUnit");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientUnitConversion_IngredientId",
                table: "IngredientUnitConversion",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientUnitConversion_UnitToConvertId",
                table: "IngredientUnitConversion",
                column: "UnitToConvertId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Unit_BaseUnit",
                table: "Ingredient",
                column: "BaseUnit",
                principalTable: "Unit",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Unit_BaseUnit",
                table: "Ingredient");

            migrationBuilder.DropTable(
                name: "IngredientUnitConversion");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_BaseUnit",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "BaseUnit",
                table: "Ingredient");
        }
    }
}
