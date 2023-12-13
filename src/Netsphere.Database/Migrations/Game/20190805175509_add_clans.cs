using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_clans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "clans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 40, nullable: false),
                    Icon = table.Column<string>(maxLength: 8, nullable: false),
                    Description = table.Column<string>(maxLength: 40, nullable: true),
                    Area = table.Column<byte>(nullable: false),
                    Activity = table.Column<byte>(nullable: false),
                    Question1 = table.Column<string>(maxLength: 40, nullable: true),
                    Question2 = table.Column<string>(maxLength: 40, nullable: true),
                    Question3 = table.Column<string>(maxLength: 40, nullable: true),
                    Question4 = table.Column<string>(maxLength: 40, nullable: true),
                    Question5 = table.Column<string>(maxLength: 40, nullable: true),
                    Class = table.Column<byte>(nullable: false),
                    Announcement = table.Column<string>(maxLength: 40, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clans", x => x.Id);
                    
                });

            migrationBuilder.CreateTable(
                name: "clan_members",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClanId = table.Column<int>(nullable: false),
                    PlayerId = table.Column<int>(nullable: false),
                    JoinDate = table.Column<long>(nullable: false),
                    State = table.Column<byte>(nullable: false),
                    Role = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clan_members", x => x.Id);

                });

            migrationBuilder.CreateIndex(
                name: "IX_clan_members_ClanId",
                table: "clan_members",
                column: "ClanId");

            migrationBuilder.CreateIndex(
                name: "IX_clan_members_PlayerId",
                table: "clan_members",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clans_Name",
                table: "clans",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_clans_OwnerId",
                table: "clans",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clan_members");

            migrationBuilder.DropTable(
                name: "clans");
        }
    }
}
