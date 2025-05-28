using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversitySystem2.Migrations
{
    public partial class ChangeLessonTimeToNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Schedules");

            migrationBuilder.AddColumn<int>(
                name: "LessonNumber",
                table: "Schedules",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LessonNumber",
                table: "Schedules");

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "Schedules",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
