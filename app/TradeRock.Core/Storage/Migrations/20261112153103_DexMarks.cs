using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class DexMarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DexMarks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: false),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    FormId = table.Column<int>(type: "INTEGER", nullable: false),
                    Shiny = table.Column<bool>(type: "INTEGER", nullable: false),
                    MarkedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DexMarks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DexMarks_OwnerId_Scope_SpeciesId_FormId_Shiny",
                table: "DexMarks",
                columns: new[] { "OwnerId", "Scope", "SpeciesId", "FormId", "Shiny" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DexMarks");
        }
    }
}
