using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommuteAlert.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWatchLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiteName",
                table: "Watches",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TargetName",
                table: "Watches",
                type: "nvarchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteName",
                table: "Watches");

            migrationBuilder.DropColumn(
                name: "TargetName",
                table: "Watches");
        }
    }
}
