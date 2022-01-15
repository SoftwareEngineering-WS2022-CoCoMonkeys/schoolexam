using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddRemarkPdfFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubmissionPdfFile_SubmissionId",
                table: "File",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_File_SubmissionPdfFile_SubmissionId",
                table: "File",
                column: "SubmissionPdfFile_SubmissionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_File_Submission_SubmissionPdfFile_SubmissionId",
                table: "File",
                column: "SubmissionPdfFile_SubmissionId",
                principalTable: "Submission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Submission_SubmissionPdfFile_SubmissionId",
                table: "File");

            migrationBuilder.DropIndex(
                name: "IX_File_SubmissionPdfFile_SubmissionId",
                table: "File");

            migrationBuilder.DropColumn(
                name: "SubmissionPdfFile_SubmissionId",
                table: "File");

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");
        }
    }
}
