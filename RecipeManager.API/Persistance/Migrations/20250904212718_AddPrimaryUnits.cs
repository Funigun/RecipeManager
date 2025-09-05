using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeManager.Api.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddPrimaryUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConversionFactor",
                table: "Unit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "PrimaryUnit",
                table: "Unit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Unit_PrimaryUnit",
                table: "Unit",
                column: "PrimaryUnit");

            migrationBuilder.AddForeignKey(
                name: "FK_Unit_Unit_PrimaryUnit",
                table: "Unit",
                column: "PrimaryUnit",
                principalTable: "Unit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Unit_Unit_PrimaryUnit",
                table: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_Unit_PrimaryUnit",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "ConversionFactor",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "PrimaryUnit",
                table: "Unit");
        }
    }
}
