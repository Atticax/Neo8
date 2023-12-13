using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_clan_events : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_clan_bans_players_BannedById",
                table: "clan_bans");

            migrationBuilder.DropIndex(
                name: "IX_clan_bans_BannedById",
                table: "clan_bans");

            migrationBuilder.DropColumn(
                name: "BannedById",
                table: "clan_bans");

            migrationBuilder.CreateTable(
                name: "clan_events",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClanId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    Date = table.Column<long>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Value1 = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clan_events", x => x.Id);

                });

            migrationBuilder.CreateIndex(
                name: "IX_clan_events_ClanId",
                table: "clan_events",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_clan_events_PlayerId",
                table: "clan_events",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clan_events");

            migrationBuilder.AddColumn<int>(
                name: "BannedById",
                table: "clan_bans",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_clan_bans_BannedById",
                table: "clan_bans",
                column: "BannedById");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_clan_bans_players_BannedById",
            //    table: "clan_bans",
            //    column: "BannedById",
            //    principalTable: "players",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
