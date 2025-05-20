using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TraitorGator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrentQuestinonToGameRoound : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_GameRounds_GameRoundId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_GameRoundId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "GameRoundId",
                table: "Questions");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentQuestionId",
                table: "GameRounds",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameRounds_CurrentQuestionId",
                table: "GameRounds",
                column: "CurrentQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameRounds_Questions_CurrentQuestionId",
                table: "GameRounds",
                column: "CurrentQuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameRounds_Questions_CurrentQuestionId",
                table: "GameRounds");

            migrationBuilder.DropIndex(
                name: "IX_GameRounds_CurrentQuestionId",
                table: "GameRounds");

            migrationBuilder.DropColumn(
                name: "CurrentQuestionId",
                table: "GameRounds");

            migrationBuilder.AddColumn<Guid>(
                name: "GameRoundId",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GameRoundId",
                table: "Questions",
                column: "GameRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_GameRounds_GameRoundId",
                table: "Questions",
                column: "GameRoundId",
                principalTable: "GameRounds",
                principalColumn: "Id");
        }
    }
}
