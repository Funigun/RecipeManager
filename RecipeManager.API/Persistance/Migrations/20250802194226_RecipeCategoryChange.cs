using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class RecipeCategoryChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeCategory_RecipeCategory_ParentId",
                table: "RecipeCategory");

            migrationBuilder.DropIndex(
                name: "IX_RecipeCategory_ParentId",
                table: "RecipeCategory");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "RecipeCategory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "RecipeCategory",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecipeCategory_ParentId",
                table: "RecipeCategory",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeCategory_RecipeCategory_ParentId",
                table: "RecipeCategory",
                column: "ParentId",
                principalTable: "RecipeCategory",
                principalColumn: "Id");
        }
    }
}
