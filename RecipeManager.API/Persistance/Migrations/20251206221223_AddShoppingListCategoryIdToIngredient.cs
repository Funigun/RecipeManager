using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddShoppingListCategoryIdToIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShoppingListCategoryId",
                table: "Ingredient",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_ShoppingListCategoryId",
                table: "Ingredient",
                column: "ShoppingListCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_IngredientCategory_ShoppingListCategoryId",
                table: "Ingredient",
                column: "ShoppingListCategoryId",
                principalTable: "IngredientCategory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_IngredientCategory_ShoppingListCategoryId",
                table: "Ingredient");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_ShoppingListCategoryId",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "ShoppingListCategoryId",
                table: "Ingredient");
        }
    }
}
