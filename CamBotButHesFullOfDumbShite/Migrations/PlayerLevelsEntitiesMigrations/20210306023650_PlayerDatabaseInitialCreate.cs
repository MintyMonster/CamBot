using Microsoft.EntityFrameworkCore.Migrations;

namespace CamBotButHesFullOfDumbShite.Migrations.PlayerLevelsEntitiesMigrations
{
    public partial class PlayerDatabaseInitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "playerLevelsModel",
                columns: table => new
                {
                    playerId = table.Column<string>(type: "TEXT", nullable: false),
                    playerUsername = table.Column<string>(type: "TEXT", nullable: true),
                    points = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_playerLevelsModel", x => x.playerId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "playerLevelsModel");
        }
    }
}
