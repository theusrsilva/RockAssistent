using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class SaveAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SaveAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OwnerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PokemonId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SavePath = table.Column<string>(type: "TEXT", nullable: false),
                    SaveSha256 = table.Column<string>(type: "TEXT", nullable: false),
                    Box = table.Column<int>(type: "INTEGER", nullable: false),
                    Slot = table.Column<int>(type: "INTEGER", nullable: false),
                    AttachedAt = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaveAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaveAttachments_Pokemon_PokemonId",
                        column: x => x.PokemonId,
                        principalTable: "Pokemon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaveAttachments_OwnerId_SavePath",
                table: "SaveAttachments",
                columns: new[] { "OwnerId", "SavePath" });

            migrationBuilder.CreateIndex(
                name: "IX_SaveAttachments_PokemonId",
                table: "SaveAttachments",
                column: "PokemonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaveAttachments");
        }
    }
}
