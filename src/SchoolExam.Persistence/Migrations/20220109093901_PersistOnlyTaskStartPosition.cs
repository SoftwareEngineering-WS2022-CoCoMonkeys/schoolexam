using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class PersistOnlyTaskStartPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndPage",
                table: "ExamTask");

            migrationBuilder.DropColumn(
                name: "EndY",
                table: "ExamTask");

            migrationBuilder.RenameColumn(
                name: "StartY",
                table: "ExamTask",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "StartPage",
                table: "ExamTask",
                newName: "Page");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ExamTask",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "ExamTask");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "ExamTask",
                newName: "StartY");

            migrationBuilder.RenameColumn(
                name: "Page",
                table: "ExamTask",
                newName: "StartPage");

            migrationBuilder.AddColumn<int>(
                name: "EndPage",
                table: "ExamTask",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "EndY",
                table: "ExamTask",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
