using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class IngredientCategoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientCategory_IngredientCategory_ParentId",
                table: "IngredientCategory");

            migrationBuilder.DropIndex(
                name: "IX_IngredientCategory_ParentId",
                table: "IngredientCategory");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "IngredientCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "IngredientCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientCategory_ParentId",
                table: "IngredientCategory",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientCategory_IngredientCategory_ParentId",
                table: "IngredientCategory",
                column: "ParentId",
                principalTable: "IngredientCategory",
                principalColumn: "Id");
        }
    }
}
