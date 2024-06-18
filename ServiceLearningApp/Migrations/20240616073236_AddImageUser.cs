using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FkImageId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FkImageId",
                table: "AspNetUsers");
        }
    }
}
