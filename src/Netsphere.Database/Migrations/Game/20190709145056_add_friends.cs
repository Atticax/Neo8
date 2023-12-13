using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_friends : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_friends",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<int>(nullable: false),
                    FriendPlayerId = table.Column<int>(nullable: false),
                    State = table.Column<byte>(nullable: false)
                 },
                constraints: table =>
            //   {
            //    table.PrimaryKey("PK_player_friends", x => x.Id);
            //    table.ForeignKey(
            //        name: "FK_player_friends_players_FriendPlayerId",
            //        column: x => x.FriendPlayerId,
            //        principalTable: "players",
            //        principalColumn: "Id",
            //        onDelete: ReferentialAction.Cascade);
            //    table.ForeignKey(
            //        name: "FK_player_friends_players_PlayerId",
            //        column: x => x.PlayerId,
            //        principalTable: "players",
            //        principalColumn: "Id",
            //       onDelete: ReferentialAction.Cascade);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_player_friends_FriendPlayerId",
                table: "player_friends",
                column: "FriendPlayerId"));

            migrationBuilder.CreateIndex(
                name: "IX_player_friends_PlayerId",
                table: "player_friends",
                column: "PlayerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_friends");
        }
    }
}
