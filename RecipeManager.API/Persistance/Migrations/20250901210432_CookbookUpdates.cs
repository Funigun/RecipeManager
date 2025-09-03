using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class CookbookUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CookbookCategoryToRecipe_Recipe_RecipesId",
                table: "CookbookCategoryToRecipe");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CookbookCategoryToRecipe",
                table: "CookbookCategoryToRecipe");

            migrationBuilder.DropIndex(
                name: "IX_CookbookCategoryToRecipe_RecipesId",
                table: "CookbookCategoryToRecipe");

            migrationBuilder.RenameColumn(
                name: "RecipesId",
                table: "CookbookCategoryToRecipe",
                newName: "Value");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CookbookCategoryToRecipe",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Cookbook",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CookbookCategoryToRecipe",
                table: "CookbookCategoryToRecipe",
                columns: new[] { "CookbookCategoryId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CookbookCategoryToRecipe",
                table: "CookbookCategoryToRecipe");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CookbookCategoryToRecipe");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Cookbook");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CookbookCategoryToRecipe",
                newName: "RecipesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CookbookCategoryToRecipe",
                table: "CookbookCategoryToRecipe",
                columns: new[] { "CookbookCategoryId", "RecipesId" });

            migrationBuilder.CreateIndex(
                name: "IX_CookbookCategoryToRecipe_RecipesId",
                table: "CookbookCategoryToRecipe",
                column: "RecipesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CookbookCategoryToRecipe_Recipe_RecipesId",
                table: "CookbookCategoryToRecipe",
                column: "RecipesId",
                principalTable: "Recipe",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
