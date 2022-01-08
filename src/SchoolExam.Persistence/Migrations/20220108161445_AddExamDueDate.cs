using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddExamDueDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Exam",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<double>(
                name: "AchievedPoints",
                table: "Answer",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_BookletId",
                table: "Submission",
                column: "BookletId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_ExamBooklet_BookletId",
                table: "Submission",
                column: "BookletId",
                principalTable: "ExamBooklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_ExamBooklet_BookletId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_BookletId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Exam");

            migrationBuilder.AlterColumn<double>(
                name: "AchievedPoints",
                table: "Answer",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
