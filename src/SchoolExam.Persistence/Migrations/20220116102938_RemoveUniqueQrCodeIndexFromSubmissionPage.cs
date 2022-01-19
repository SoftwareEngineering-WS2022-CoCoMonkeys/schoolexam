using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class RemoveUniqueQrCodeIndexFromSubmissionPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage",
                column: "QrCodeData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage",
                column: "QrCodeData",
                unique: true);
        }
    }
}
