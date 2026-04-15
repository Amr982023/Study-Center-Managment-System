using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixGroupReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSessions_Groups_GroupId1",
                table: "ClassSessions");

            migrationBuilder.DropIndex(
                name: "IX_ClassSessions_GroupId1",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "GroupId1",
                table: "ClassSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId1",
                table: "ClassSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSessions_GroupId1",
                table: "ClassSessions",
                column: "GroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSessions_Groups_GroupId1",
                table: "ClassSessions",
                column: "GroupId1",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
