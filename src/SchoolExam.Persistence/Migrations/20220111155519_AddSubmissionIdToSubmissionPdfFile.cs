using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddSubmissionIdToSubmissionPdfFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submission_ExamBooklet_ExamBookletId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_BookletId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_Submission_ExamBookletId",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "ExamBookletId",
                table: "Submission");

            migrationBuilder.AlterColumn<double>(
                name: "MaxPoints",
                table: "ExamTask",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionId",
                table: "ExamBooklet",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_BookletId",
                table: "Submission",
                column: "BookletId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBooklet_SubmissionId",
                table: "ExamBooklet",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamBooklet_Submission_SubmissionId",
                table: "ExamBooklet",
                column: "SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamBooklet_Submission_SubmissionId",
                table: "ExamBooklet");

            migrationBuilder.DropIndex(
                name: "IX_Submission_BookletId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_ExamBooklet_SubmissionId",
                table: "ExamBooklet");

            migrationBuilder.DropColumn(
                name: "SubmissionId",
                table: "ExamBooklet");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamBookletId",
                table: "Submission",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaxPoints",
                table: "ExamTask",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_BookletId",
                table: "Submission",
                column: "BookletId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ExamBookletId",
                table: "Submission",
                column: "ExamBookletId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_ExamBooklet_ExamBookletId",
                table: "Submission",
                column: "ExamBookletId",
                principalTable: "ExamBooklet",
                principalColumn: "Id");
        }
    }
}
