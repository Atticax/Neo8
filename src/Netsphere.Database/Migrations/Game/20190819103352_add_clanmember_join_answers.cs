using Microsoft.EntityFrameworkCore.Migrations;

namespace Netsphere.Database.Migrations.Game
{
    public partial class add_clanmember_join_answers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Answer1",
                table: "clan_members",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answer2",
                table: "clan_members",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answer3",
                table: "clan_members",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answer4",
                table: "clan_members",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Answer5",
                table: "clan_members",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Answer1",
                table: "clan_members");

            migrationBuilder.DropColumn(
                name: "Answer2",
                table: "clan_members");

            migrationBuilder.DropColumn(
                name: "Answer3",
                table: "clan_members");

            migrationBuilder.DropColumn(
                name: "Answer4",
                table: "clan_members");

            migrationBuilder.DropColumn(
                name: "Answer5",
                table: "clan_members");
        }
    }
}
