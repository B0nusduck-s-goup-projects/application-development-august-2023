using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM2.Data.Migrations
{
    /// <inheritdoc />
    public partial class ASM2_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "CartItem",
                newName: "TotalPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "CartItem",
                newName: "UnitPrice");
        }
    }
}
