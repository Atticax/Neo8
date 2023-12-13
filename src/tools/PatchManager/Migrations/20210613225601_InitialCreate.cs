using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PatchManager.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "managed_program",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_managed_program", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "program_version",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ManagedProgramId = table.Column<int>(nullable: false),
                    Major = table.Column<int>(nullable: false),
                    Minor = table.Column<int>(nullable: false),
                    Patch = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_program_version", x => x.Id);
                    table.ForeignKey(
                        name: "FK_program_version_managed_program_ManagedProgramId",
                        column: x => x.ManagedProgramId,
                        principalTable: "managed_program",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "patch",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ManagedProgramId = table.Column<int>(nullable: false),
                    FromVersionId = table.Column<int>(nullable: false),
                    ToVersionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patch", x => x.Id);
                    table.ForeignKey(
                        name: "FK_patch_program_version_FromVersionId",
                        column: x => x.FromVersionId,
                        principalTable: "program_version",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_patch_managed_program_ManagedProgramId",
                        column: x => x.ManagedProgramId,
                        principalTable: "managed_program",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_patch_program_version_ToVersionId",
                        column: x => x.ToVersionId,
                        principalTable: "program_version",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_managed_program_Name",
                table: "managed_program",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_patch_FromVersionId",
                table: "patch",
                column: "FromVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_patch_ManagedProgramId",
                table: "patch",
                column: "ManagedProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_patch_ToVersionId",
                table: "patch",
                column: "ToVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_program_version_ManagedProgramId",
                table: "program_version",
                column: "ManagedProgramId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "patch");

            migrationBuilder.DropTable(
                name: "program_version");

            migrationBuilder.DropTable(
                name: "managed_program");
        }
    }
}
