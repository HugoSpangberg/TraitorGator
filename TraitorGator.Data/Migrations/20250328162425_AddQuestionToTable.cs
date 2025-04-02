using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraitorGator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentRound",
                table: "GameRounds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TraitorId",
                table: "GameRounds",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AlteredText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoundNumber = table.Column<int>(type: "int", nullable: false),
                    GameRoundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_GameRounds_GameRoundId",
                        column: x => x.GameRoundId,
                        principalTable: "GameRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRounds_TraitorId",
                table: "GameRounds",
                column: "TraitorId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GameRoundId",
                table: "Questions",
                column: "GameRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRounds_Players_TraitorId",
                table: "GameRounds",
                column: "TraitorId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRounds_Players_TraitorId",
                table: "GameRounds");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_GameRounds_TraitorId",
                table: "GameRounds");

            migrationBuilder.DropColumn(
                name: "CurrentRound",
                table: "GameRounds");

            migrationBuilder.DropColumn(
                name: "TraitorId",
                table: "GameRounds");
        }
    }
}
