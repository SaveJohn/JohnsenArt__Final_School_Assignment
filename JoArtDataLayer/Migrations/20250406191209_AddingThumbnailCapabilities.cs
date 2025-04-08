using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JoArtDataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddingThumbnailCapabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailKey",
                table: "ArtworkImages",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailKey",
                table: "ArtworkImages");
        }
    }
}
