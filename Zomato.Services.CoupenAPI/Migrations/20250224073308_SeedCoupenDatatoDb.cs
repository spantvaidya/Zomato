using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zomato.Services.CoupenAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCoupenDatatoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Coupens",
                columns: new[] { "CoupenId", "CoupenCode", "CreatedDate", "DiscountAmount", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "10", "50" },
                    { 2, "20OFF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "20", "80" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Coupens",
                keyColumn: "CoupenId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Coupens",
                keyColumn: "CoupenId",
                keyValue: 2);
        }
    }
}
