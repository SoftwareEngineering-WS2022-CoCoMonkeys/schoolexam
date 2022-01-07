using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class ChangeBookletSubmissionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamBookletPage_ExamBooklet_ExamBookletId",
                table: "ExamBookletPage");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Person_StudentId",
                table: "Submission");

            migrationBuilder.DropIndex(
                name: "IX_ExamBookletPage_QrCode",
                table: "ExamBookletPage");

            migrationBuilder.DropColumn(
                name: "Page",
                table: "SubmissionPage");

            migrationBuilder.DropColumn(
                name: "ScanData",
                table: "SubmissionPage");

            migrationBuilder.DropColumn(
                name: "HasScanner",
                table: "School");

            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "ExamBookletPage");

            migrationBuilder.RenameColumn(
                name: "ExamBookletId",
                table: "ExamBookletPage",
                newName: "BookletId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamBookletPage_ExamBookletId",
                table: "ExamBookletPage",
                newName: "IX_ExamBookletPage_BookletId");

            migrationBuilder.AddColumn<Guid>(
                name: "BookletPageId",
                table: "SubmissionPage",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ExamId",
                table: "SubmissionPage",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "Submission",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "QrCodeData",
                table: "ExamBookletPage",
                type: "character(32)",
                fixedLength: true,
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "ExamBooklet",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Exam",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Exam",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Exam",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Course",
                type: "integer",
                nullable: false,
                defaultValue: 2022,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2021);

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UploaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<byte[]>(type: "bytea", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    BookletId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmissionPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_ExamBooklet_BookletId",
                        column: x => x.BookletId,
                        principalTable: "ExamBooklet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_SubmissionPage_SubmissionPageId",
                        column: x => x.SubmissionPageId,
                        principalTable: "SubmissionPage",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_User_UploaderId",
                        column: x => x.UploaderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_BookletPageId",
                table: "SubmissionPage",
                column: "BookletPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_ExamId",
                table: "SubmissionPage",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBookletPage_QrCodeData",
                table: "ExamBookletPage",
                column: "QrCodeData",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_BookletId",
                table: "File",
                column: "BookletId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_ExamId",
                table: "File",
                column: "ExamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_SubmissionId",
                table: "File",
                column: "SubmissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_SubmissionPageId",
                table: "File",
                column: "SubmissionPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_UploaderId",
                table: "File",
                column: "UploaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamBookletPage_ExamBooklet_BookletId",
                table: "ExamBookletPage",
                column: "BookletId",
                principalTable: "ExamBooklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Person_StudentId",
                table: "Submission",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPage_Exam_ExamId",
                table: "SubmissionPage",
                column: "ExamId",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPage_ExamBookletPage_BookletPageId",
                table: "SubmissionPage",
                column: "BookletPageId",
                principalTable: "ExamBookletPage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamBookletPage_ExamBooklet_BookletId",
                table: "ExamBookletPage");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Person_StudentId",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPage_Exam_ExamId",
                table: "SubmissionPage");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPage_ExamBookletPage_BookletPageId",
                table: "SubmissionPage");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionPage_BookletPageId",
                table: "SubmissionPage");

            migrationBuilder.DropIndex(
                name: "IX_SubmissionPage_ExamId",
                table: "SubmissionPage");

            migrationBuilder.DropIndex(
                name: "IX_ExamBookletPage_QrCodeData",
                table: "ExamBookletPage");

            migrationBuilder.DropColumn(
                name: "BookletPageId",
                table: "SubmissionPage");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "SubmissionPage");

            migrationBuilder.DropColumn(
                name: "QrCodeData",
                table: "ExamBookletPage");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "ExamBooklet");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Exam");

            migrationBuilder.RenameColumn(
                name: "BookletId",
                table: "ExamBookletPage",
                newName: "ExamBookletId");

            migrationBuilder.RenameIndex(
                name: "IX_ExamBookletPage_BookletId",
                table: "ExamBookletPage",
                newName: "IX_ExamBookletPage_ExamBookletId");

            migrationBuilder.AddColumn<int>(
                name: "Page",
                table: "SubmissionPage",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte[]>(
                name: "ScanData",
                table: "SubmissionPage",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<Guid>(
                name: "StudentId",
                table: "Submission",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasScanner",
                table: "School",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "QrCode",
                table: "ExamBookletPage",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<int>(
                name: "Year",
                table: "Course",
                type: "integer",
                nullable: false,
                defaultValue: 2021,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 2022);

            migrationBuilder.CreateIndex(
                name: "IX_ExamBookletPage_QrCode",
                table: "ExamBookletPage",
                column: "QrCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamBookletPage_ExamBooklet_ExamBookletId",
                table: "ExamBookletPage",
                column: "ExamBookletId",
                principalTable: "ExamBooklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Person_StudentId",
                table: "Submission",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
