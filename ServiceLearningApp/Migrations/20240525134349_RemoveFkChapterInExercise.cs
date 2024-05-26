using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFkChapterInExercise : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercises_Chapters_FkChapterId",
                table: "Exercises");

            migrationBuilder.DropIndex(
                name: "IX_Exercises_FkChapterId",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "FkChapterId",
                table: "Exercises");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FkChapterId",
                table: "Exercises",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_FkChapterId",
                table: "Exercises",
                column: "FkChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercises_Chapters_FkChapterId",
                table: "Exercises",
                column: "FkChapterId",
                principalTable: "Chapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
