using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class MonIsStarter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStarter",
                table: "NuzlockeMons",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            // Os iniciais automáticos já criados: o encontro deles foi gravado com "starter": true.
            migrationBuilder.Sql("""
                UPDATE NuzlockeMons SET IsStarter = 1
                WHERE EXISTS (
                    SELECT 1 FROM NuzlockeEvents e
                    WHERE e.RunId = NuzlockeMons.RunId AND e.Kind = 'encounter' AND json_extract(e.Json, '$.starter') = 1
                      AND upper(json_extract(e.Json, '$.monId')) = upper(NuzlockeMons.Id))
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStarter",
                table: "NuzlockeMons");
        }
    }
}
