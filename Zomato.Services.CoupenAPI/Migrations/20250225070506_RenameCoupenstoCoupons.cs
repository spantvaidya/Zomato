using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zomato.Services.CoupenAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameCoupenstoCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupens");

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    CouponId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CouponCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.CouponId);
                });

            migrationBuilder.InsertData(
                table: "Coupons",
                columns: new[] { "CouponId", "CouponCode", "CreatedDate", "DiscountAmount", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "10", "50" },
                    { 2, "20OFF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "20", "80" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.CreateTable(
                name: "Coupens",
                columns: table => new
                {
                    CoupenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoupenCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiscountAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupens", x => x.CoupenId);
                });

            migrationBuilder.InsertData(
                table: "Coupens",
                columns: new[] { "CoupenId", "CoupenCode", "CreatedDate", "DiscountAmount", "MinAmount" },
                values: new object[,]
                {
                    { 1, "10OFF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "10", "50" },
                    { 2, "20OFF", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "20", "80" }
                });
        }
    }
}
