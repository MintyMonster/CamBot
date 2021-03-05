using Microsoft.EntityFrameworkCore.Migrations;

namespace CamBotButHesFullOfDumbShite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "serverConfigModel",
                columns: table => new
                {
                    guildid = table.Column<string>(type: "TEXT", nullable: false),
                    prefix = table.Column<string>(type: "TEXT", nullable: true),
                    color = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serverConfigModel", x => x.guildid);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "serverConfigModel");
        }
    }
}
