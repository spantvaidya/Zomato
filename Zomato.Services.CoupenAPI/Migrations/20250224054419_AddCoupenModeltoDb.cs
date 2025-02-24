using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zomato.Services.CoupenAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCoupenModeltoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupens",
                columns: table => new
                {
                    CoupenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoupenCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MinAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupens", x => x.CoupenId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coupens");
        }
    }
}
