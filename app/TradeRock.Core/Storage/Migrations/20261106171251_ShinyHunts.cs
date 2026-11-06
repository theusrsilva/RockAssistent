using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class ShinyHunts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hunts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Species = table.Column<int>(type: "INTEGER", nullable: false),
                    Form = table.Column<int>(type: "INTEGER", nullable: false),
                    GameKey = table.Column<string>(type: "TEXT", nullable: false),
                    MethodId = table.Column<string>(type: "TEXT", nullable: false),
                    ModifiersJson = table.Column<string>(type: "TEXT", nullable: false),
                    Count = table.Column<int>(type: "INTEGER", nullable: false),
                    ElapsedSeconds = table.Column<long>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<long>(type: "INTEGER", nullable: false),
                    EndedAt = table.Column<long>(type: "INTEGER", nullable: true),
                    Found = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hunts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hunts_OwnerId_StartedAt",
                table: "Hunts",
                columns: new[] { "OwnerId", "StartedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hunts");
        }
    }
}
