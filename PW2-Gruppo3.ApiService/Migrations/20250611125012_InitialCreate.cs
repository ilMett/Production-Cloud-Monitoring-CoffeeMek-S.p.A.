using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PW2Gruppo3.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VatCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Batches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemQuantity = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Batches_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Batches_Sites_SiteId",
                        column: x => x.SiteId,
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssemblyLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AverageStationTime = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperatorsNumber = table.Column<int>(type: "int", nullable: false),
                    Faults = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFirst = table.Column<bool>(type: "bit", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: false),
                    MachineBlockage = table.Column<bool>(type: "bit", nullable: false),
                    BlockageCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampLocal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssemblyLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssemblyLines_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lathes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MachineState = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rpm = table.Column<int>(type: "int", nullable: false),
                    SpindleTemperature = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    CompletedItemsQuantity = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFirst = table.Column<bool>(type: "bit", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: false),
                    MachineBlockage = table.Column<bool>(type: "bit", nullable: false),
                    BlockageCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampLocal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lathes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lathes_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Millings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedItemsQuantity = table.Column<int>(type: "int", nullable: false),
                    CycleDuration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CuttingDepth = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Vibration = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    UserAlerts = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFirst = table.Column<bool>(type: "bit", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: false),
                    MachineBlockage = table.Column<bool>(type: "bit", nullable: false),
                    BlockageCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampLocal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Millings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Millings_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TestResult = table.Column<bool>(type: "bit", nullable: false),
                    BoilerPressure = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    BoilerTemperature = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    EnergyConsumption = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFirst = table.Column<bool>(type: "bit", nullable: false),
                    IsLast = table.Column<bool>(type: "bit", nullable: false),
                    MachineBlockage = table.Column<bool>(type: "bit", nullable: false),
                    BlockageCause = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastMaintenance = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampLocal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestLines_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssemblyLines_BatchId",
                table: "AssemblyLines",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CustomerId",
                table: "Batches",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Batches_SiteId",
                table: "Batches",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Lathes_BatchId",
                table: "Lathes",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Millings_BatchId",
                table: "Millings",
                column: "BatchId");

            migrationBuilder.CreateIndex(
                name: "IX_TestLines_BatchId",
                table: "TestLines",
                column: "BatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssemblyLines");

            migrationBuilder.DropTable(
                name: "Lathes");

            migrationBuilder.DropTable(
                name: "Millings");

            migrationBuilder.DropTable(
                name: "TestLines");

            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Sites");
        }
    }
}
