using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RecipeToCategoryConfigUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeCategory_Recipe_RecipeId",
                table: "RecipeCategory");

            migrationBuilder.DropIndex(
                name: "IX_RecipeCategory_RecipeId",
                table: "RecipeCategory");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "RecipeCategory");

            migrationBuilder.AlterColumn<double>(
                name: "Amount",
                table: "RecipeIngredient",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateTable(
                name: "RecipeToRecipeCategory",
                columns: table => new
                {
                    RecipeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeToRecipeCategory", x => new { x.RecipeId, x.Id });
                    table.ForeignKey(
                        name: "FK_RecipeToRecipeCategory_Recipe_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecipeToRecipeCategory");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "RecipeIngredient",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<Guid>(
                name: "RecipeId",
                table: "RecipeCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategory_RecipeId",
                table: "RecipeCategory",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeCategory_Recipe_RecipeId",
                table: "RecipeCategory",
                column: "RecipeId",
                principalTable: "Recipe",
                principalColumn: "Id");
        }
    }
}
