using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseUnitAndConversionsToIngredientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BaseUnit",
                table: "Ingredient",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "IngredientUnitConvertions",
                table: "Ingredient",
                type: "json",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_BaseUnit",
                table: "Ingredient",
                column: "BaseUnit");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Unit_BaseUnit",
                table: "Ingredient",
                column: "BaseUnit",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Unit_BaseUnit",
                table: "Ingredient");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_BaseUnit",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "BaseUnit",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "IngredientUnitConvertions",
                table: "Ingredient");
        }
    }
}
