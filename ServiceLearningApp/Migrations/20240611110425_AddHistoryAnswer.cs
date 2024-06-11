using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryAnswer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoryAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FkQuestionId = table.Column<int>(type: "integer", nullable: false),
                    FkOptionId = table.Column<int>(type: "integer", nullable: false),
                    FkExerciseTransactionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoryAnswers_ExerciseTransactions_FkExerciseTransactionId",
                        column: x => x.FkExerciseTransactionId,
                        principalTable: "ExerciseTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoryAnswers_Options_FkOptionId",
                        column: x => x.FkOptionId,
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HistoryAnswers_Questions_FkQuestionId",
                        column: x => x.FkQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoryAnswers_FkExerciseTransactionId",
                table: "HistoryAnswers",
                column: "FkExerciseTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryAnswers_FkOptionId",
                table: "HistoryAnswers",
                column: "FkOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoryAnswers_FkQuestionId",
                table: "HistoryAnswers",
                column: "FkQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoryAnswers");
        }
    }
}
