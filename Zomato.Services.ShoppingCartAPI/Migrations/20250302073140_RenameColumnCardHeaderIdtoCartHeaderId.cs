using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zomato.Services.CartAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnCardHeaderIdtoCartHeaderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CardHeaderId",
                table: "CartHeaders",
                newName: "CartHeaderId");

            migrationBuilder.RenameColumn(
                name: "CardDetailsId",
                table: "CartDetails",
                newName: "CartDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CartHeaderId",
                table: "CartHeaders",
                newName: "CardHeaderId");

            migrationBuilder.RenameColumn(
                name: "CartDetailsId",
                table: "CartDetails",
                newName: "CardDetailsId");
        }
    }
}
