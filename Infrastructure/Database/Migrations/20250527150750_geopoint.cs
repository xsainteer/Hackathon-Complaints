using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class geopoint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Submissions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Submissions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Authorities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Authorities");
        }
    }
}
