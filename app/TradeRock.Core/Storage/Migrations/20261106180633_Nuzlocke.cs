using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Nuzlocke : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NuzlockeRuns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    GameKey = table.Column<string>(type: "TEXT", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: true),
                    RulesJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    StartedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    EndedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    CurrentAreaId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NuzlockeRuns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NuzlockeEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RunId = table.Column<Guid>(type: "TEXT", nullable: false),
                    At = table.Column<long>(type: "INTEGER", nullable: false),
                    Kind = table.Column<string>(type: "TEXT", nullable: false),
                    Json = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NuzlockeEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NuzlockeEvents_NuzlockeRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "NuzlockeRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NuzlockeMons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RunId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AreaId = table.Column<string>(type: "TEXT", nullable: false),
                    Species = table.Column<int>(type: "INTEGER", nullable: false),
                    Form = table.Column<int>(type: "INTEGER", nullable: false),
                    Nickname = table.Column<string>(type: "TEXT", nullable: true),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DeathCause = table.Column<string>(type: "TEXT", nullable: true),
                    CaughtAt = table.Column<long>(type: "INTEGER", nullable: false),
                    DiedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    MovesJson = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NuzlockeMons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NuzlockeMons_NuzlockeRuns_RunId",
                        column: x => x.RunId,
                        principalTable: "NuzlockeRuns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NuzlockeEvents_RunId_At",
                table: "NuzlockeEvents",
                columns: new[] { "RunId", "At" });

            migrationBuilder.CreateIndex(
                name: "IX_NuzlockeMons_RunId",
                table: "NuzlockeMons",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_NuzlockeRuns_OwnerId_StartedAt",
                table: "NuzlockeRuns",
                columns: new[] { "OwnerId", "StartedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NuzlockeEvents");

            migrationBuilder.DropTable(
                name: "NuzlockeMons");

            migrationBuilder.DropTable(
                name: "NuzlockeRuns");
        }
    }
}
