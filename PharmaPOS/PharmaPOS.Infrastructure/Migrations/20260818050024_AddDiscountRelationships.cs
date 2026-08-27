using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaPOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicableDrugIds",
                table: "Discounts");

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountId",
                table: "Sales",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DrugDiscounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DrugId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiscountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrugDiscounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrugDiscounts_Discounts_DiscountId",
                        column: x => x.DiscountId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrugDiscounts_Drugs_DrugId",
                        column: x => x.DrugId,
                        principalTable: "Drugs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_DiscountId",
                table: "Sales",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_DrugDiscounts_DiscountId",
                table: "DrugDiscounts",
                column: "DiscountId");

            migrationBuilder.CreateIndex(
                name: "IX_DrugDiscounts_DrugId_DiscountId",
                table: "DrugDiscounts",
                columns: new[] { "DrugId", "DiscountId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Discounts_DiscountId",
                table: "Sales",
                column: "DiscountId",
                principalTable: "Discounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Discounts_DiscountId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "DrugDiscounts");

            migrationBuilder.DropIndex(
                name: "IX_Sales_DiscountId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DiscountId",
                table: "Sales");

            migrationBuilder.AddColumn<string>(
                name: "ApplicableDrugIds",
                table: "Discounts",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
