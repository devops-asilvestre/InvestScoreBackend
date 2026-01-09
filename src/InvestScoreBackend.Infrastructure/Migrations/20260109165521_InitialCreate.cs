using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvestScoreBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetHeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileRecordId = table.Column<int>(type: "int", nullable: false),
                    FileRecordId1 = table.Column<int>(type: "int", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    MachineIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetHeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetHeads_FileRecords_FileRecordId",
                        column: x => x.FileRecordId,
                        principalTable: "FileRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetHeads_FileRecords_FileRecordId1",
                        column: x => x.FileRecordId1,
                        principalTable: "FileRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ROE = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    DY = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    CAGR = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    Liquidez = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    Risco = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<double>(type: "float(18)", precision: 18, scale: 2, nullable: false),
                    AssetHeadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_AssetHeads_AssetHeadId",
                        column: x => x.AssetHeadId,
                        principalTable: "AssetHeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetHeads_FileRecordId",
                table: "AssetHeads",
                column: "FileRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetHeads_FileRecordId1",
                table: "AssetHeads",
                column: "FileRecordId1");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_AssetHeadId",
                table: "Assets",
                column: "AssetHeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "AssetHeads");

            migrationBuilder.DropTable(
                name: "FileRecords");
        }
    }
}
