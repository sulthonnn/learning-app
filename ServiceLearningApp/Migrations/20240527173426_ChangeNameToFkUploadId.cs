using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameToFkUploadId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubChapters_Uploads_FkImageId",
                table: "SubChapters");

            migrationBuilder.RenameColumn(
                name: "FkImageId",
                table: "SubChapters",
                newName: "FkUploadId");

            migrationBuilder.RenameIndex(
                name: "IX_SubChapters_FkImageId",
                table: "SubChapters",
                newName: "IX_SubChapters_FkUploadId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubChapters_Uploads_FkUploadId",
                table: "SubChapters",
                column: "FkUploadId",
                principalTable: "Uploads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubChapters_Uploads_FkUploadId",
                table: "SubChapters");

            migrationBuilder.RenameColumn(
                name: "FkUploadId",
                table: "SubChapters",
                newName: "FkImageId");

            migrationBuilder.RenameIndex(
                name: "IX_SubChapters_FkUploadId",
                table: "SubChapters",
                newName: "IX_SubChapters_FkImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubChapters_Uploads_FkImageId",
                table: "SubChapters",
                column: "FkImageId",
                principalTable: "Uploads",
                principalColumn: "Id");
        }
    }
}
