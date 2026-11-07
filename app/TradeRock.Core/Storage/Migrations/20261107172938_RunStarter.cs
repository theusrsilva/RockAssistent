using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeRock.Core.Storage.Migrations
{
    /// <inheritdoc />
    public partial class RunStarter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Starter",
                table: "NuzlockeRuns",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Starter",
                table: "NuzlockeRuns");
        }
    }
}
