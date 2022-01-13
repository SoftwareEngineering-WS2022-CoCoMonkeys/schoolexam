using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddStudentQrCodeToSubmissionPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCodeData",
                table: "SubmissionPage",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage",
                column: "QrCodeData",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage");

            migrationBuilder.DropColumn(
                name: "QrCodeData",
                table: "SubmissionPage");

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");
        }
    }
}
