using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class ActivityRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivityRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    SubKind = table.Column<string>(type: "TEXT", nullable: true),
                    Game = table.Column<string>(type: "TEXT", nullable: false),
                    EngineId = table.Column<string>(type: "TEXT", nullable: false),
                    IsSimulation = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    EndedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Ok = table.Column<bool>(type: "INTEGER", nullable: false),
                    ErrorCode = table.Column<string>(type: "TEXT", nullable: true),
                    Label = table.Column<string>(type: "TEXT", nullable: true),
                    Species = table.Column<int>(type: "INTEGER", nullable: true),
                    SummaryJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityRecords_OwnerId_StartedAt",
                table: "ActivityRecords",
                columns: new[] { "OwnerId", "StartedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityRecords");
        }
    }
}
