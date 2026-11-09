using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <summary>
    /// D34: a coluna Family (jogo do catálogo de troca) vira Format (formato do PKHeX, de Pk1 a Pa9). Os valores
    /// antigos viram o formato de cada jogo; os Pokémon e os arquivos do cofre continuam os mesmos.
    /// </summary>
    public partial class VaultFormats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Family",
                table: "Pokemon",
                newName: "Format");

            foreach (var (family, format) in Map)
                migrationBuilder.Sql($"UPDATE \"Pokemon\" SET \"Format\" = '{format}' WHERE \"Format\" = '{family}';");
        }

        private static readonly (string Family, string Format)[] Map =
        [
            ("Frlg", "Pk3"), ("Lgpe", "Pb7"), ("Swsh", "Pk8"), ("Bdsp", "Pb8"), ("Pla", "Pa8"), ("Sv", "Pk9"), ("Za", "Pa9"),
        ];

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Formatos sem jogo no catálogo de troca (Gen 1, 2, 4 a 7) não têm para onde voltar: ficam com o nome do
            // formato, que a versão antiga do app não lê.
            foreach (var (family, format) in Map)
                migrationBuilder.Sql($"UPDATE \"Pokemon\" SET \"Format\" = '{family}' WHERE \"Format\" = '{format}';");
            migrationBuilder.RenameColumn(
                name: "Format",
                table: "Pokemon",
                newName: "Family");
        }
    }
}
