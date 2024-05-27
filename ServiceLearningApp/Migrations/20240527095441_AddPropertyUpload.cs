using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceLearningApp.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Uploads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Size",
                table: "Uploads",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "Uploads",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Uploads",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Uploads",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Uploads");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Uploads");
        }
    }
}
