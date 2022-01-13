using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class RemoveBookletSubmissionOneToManyRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_Submission_BookletId",
                table: "Submission",
                column: "BookletId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Submission_BookletId",
                table: "Submission");

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
    }
}
