using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraitorGator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStarterGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Started",
                table: "GameRounds",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Started",
                table: "GameRounds");
        }
    }
}
