using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW2Gruppo3.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class BatchQueueItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchQueueItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BatchUuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BatchQueueItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BatchQueueItems_Position",
                table: "BatchQueueItems",
                column: "Position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchQueueItems");
        }
    }
}
