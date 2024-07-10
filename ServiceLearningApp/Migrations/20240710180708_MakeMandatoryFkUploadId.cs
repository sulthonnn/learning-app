using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class MakeMandatoryFkUploadId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubChapters_Uploads_FkUploadId",
                table: "SubChapters");

            migrationBuilder.AlterColumn<int>(
                name: "FkUploadId",
                table: "SubChapters",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SubChapters_Uploads_FkUploadId",
                table: "SubChapters",
                column: "FkUploadId",
                principalTable: "Uploads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubChapters_Uploads_FkUploadId",
                table: "SubChapters");

            migrationBuilder.AlterColumn<int>(
                name: "FkUploadId",
                table: "SubChapters",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_SubChapters_Uploads_FkUploadId",
                table: "SubChapters",
                column: "FkUploadId",
                principalTable: "Uploads",
                principalColumn: "Id");
        }
    }
}
