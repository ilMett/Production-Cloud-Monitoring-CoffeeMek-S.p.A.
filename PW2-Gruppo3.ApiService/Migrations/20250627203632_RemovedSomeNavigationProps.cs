using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW2Gruppo3.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class RemovedSomeNavigationProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "TestLines",
                newName: "MachineryId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Millings",
                newName: "MachineryId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "Lathes",
                newName: "MachineryId");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "AssemblyLines",
                newName: "MachineryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MachineryId",
                table: "TestLines",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "MachineryId",
                table: "Millings",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "MachineryId",
                table: "Lathes",
                newName: "ItemId");

            migrationBuilder.RenameColumn(
                name: "MachineryId",
                table: "AssemblyLines",
                newName: "ItemId");
        }
    }
}
