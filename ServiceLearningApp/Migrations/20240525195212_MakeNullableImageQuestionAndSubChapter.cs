using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class MakeNullableImageQuestionAndSubChapter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Uploads_FkImageId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubChapters_Uploads_FkImageId",
                table: "SubChapters");

            migrationBuilder.AlterColumn<int>(
                name: "FkImageId",
                table: "SubChapters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "FkImageId",
                table: "Questions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Uploads_FkImageId",
                table: "Questions",
                column: "FkImageId",
                principalTable: "Uploads",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubChapters_Uploads_FkImageId",
                table: "SubChapters",
                column: "FkImageId",
                principalTable: "Uploads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Uploads_FkImageId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_SubChapters_Uploads_FkImageId",
                table: "SubChapters");

            migrationBuilder.AlterColumn<int>(
                name: "FkImageId",
                table: "SubChapters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "FkImageId",
                table: "Questions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Uploads_FkImageId",
                table: "Questions",
                column: "FkImageId",
                principalTable: "Uploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubChapters_Uploads_FkImageId",
                table: "SubChapters",
                column: "FkImageId",
                principalTable: "Uploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
