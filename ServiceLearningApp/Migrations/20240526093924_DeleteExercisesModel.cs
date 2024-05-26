using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class DeleteExercisesModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseTransactions_Exercises_FkExerciseId",
                table: "ExerciseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Exercises_FkExerciseId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.RenameColumn(
                name: "FkExerciseId",
                table: "Questions",
                newName: "FkSubChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_FkExerciseId",
                table: "Questions",
                newName: "IX_Questions_FkSubChapterId");

            migrationBuilder.RenameColumn(
                name: "FkExerciseId",
                table: "ExerciseTransactions",
                newName: "FkSubChapterId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseTransactions_FkExerciseId",
                table: "ExerciseTransactions",
                newName: "IX_ExerciseTransactions_FkSubChapterId");

            migrationBuilder.AlterColumn<string>(
                name: "FeedBack",
                table: "Questions",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseTransactions_SubChapters_FkSubChapterId",
                table: "ExerciseTransactions",
                column: "FkSubChapterId",
                principalTable: "SubChapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_SubChapters_FkSubChapterId",
                table: "Questions",
                column: "FkSubChapterId",
                principalTable: "SubChapters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExerciseTransactions_SubChapters_FkSubChapterId",
                table: "ExerciseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_SubChapters_FkSubChapterId",
                table: "Questions");

            migrationBuilder.RenameColumn(
                name: "FkSubChapterId",
                table: "Questions",
                newName: "FkExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_FkSubChapterId",
                table: "Questions",
                newName: "IX_Questions_FkExerciseId");

            migrationBuilder.RenameColumn(
                name: "FkSubChapterId",
                table: "ExerciseTransactions",
                newName: "FkExerciseId");

            migrationBuilder.RenameIndex(
                name: "IX_ExerciseTransactions_FkSubChapterId",
                table: "ExerciseTransactions",
                newName: "IX_ExerciseTransactions_FkExerciseId");

            migrationBuilder.AlterColumn<string>(
                name: "FeedBack",
                table: "Questions",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FkSubChapterId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_SubChapters_FkSubChapterId",
                        column: x => x.FkSubChapterId,
                        principalTable: "SubChapters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_FkSubChapterId",
                table: "Exercises",
                column: "FkSubChapterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExerciseTransactions_Exercises_FkExerciseId",
                table: "ExerciseTransactions",
                column: "FkExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Exercises_FkExerciseId",
                table: "Questions",
                column: "FkExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
