using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFkImageUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FkImageId",
                table: "AspNetUsers",
                column: "FkImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Uploads_FkImageId",
                table: "AspNetUsers",
                column: "FkImageId",
                principalTable: "Uploads",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Uploads_FkImageId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FkImageId",
                table: "AspNetUsers");
        }
    }
}
