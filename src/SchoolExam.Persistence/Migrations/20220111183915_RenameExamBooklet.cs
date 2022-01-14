using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class RenameExamBooklet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_ExamBooklet_BookletId",
                table: "File");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_ExamBooklet_BookletId",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPage_ExamBookletPage_BookletPageId",
                table: "SubmissionPage");

            migrationBuilder.DropTable(
                name: "ExamBookletPage");

            migrationBuilder.DropTable(
                name: "ExamBooklet");

            migrationBuilder.CreateTable(
                name: "Booklet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Booklet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Booklet_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookletPage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Page = table.Column<int>(type: "integer", nullable: false),
                    BookletId = table.Column<Guid>(type: "uuid", nullable: false),
                    QrCodeData = table.Column<string>(type: "character(32)", fixedLength: true, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookletPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookletPage_Booklet_BookletId",
                        column: x => x.BookletId,
                        principalTable: "Booklet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Booklet_ExamId",
                table: "Booklet",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_BookletPage_BookletId",
                table: "BookletPage",
                column: "BookletId");

            migrationBuilder.CreateIndex(
                name: "IX_BookletPage_QrCodeData",
                table: "BookletPage",
                column: "QrCodeData",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_File_Booklet_BookletId",
                table: "File",
                column: "BookletId",
                principalTable: "Booklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_Booklet_BookletId",
                table: "Submission",
                column: "BookletId",
                principalTable: "Booklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPage_BookletPage_BookletPageId",
                table: "SubmissionPage",
                column: "BookletPageId",
                principalTable: "BookletPage",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_Booklet_BookletId",
                table: "File");

            migrationBuilder.DropForeignKey(
                name: "FK_Submission_Booklet_BookletId",
                table: "Submission");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionPage_BookletPage_BookletPageId",
                table: "SubmissionPage");

            migrationBuilder.DropTable(
                name: "BookletPage");

            migrationBuilder.DropTable(
                name: "Booklet");

            migrationBuilder.CreateTable(
                name: "ExamBooklet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceNumber = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamBooklet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamBooklet_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamBookletPage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookletId = table.Column<Guid>(type: "uuid", nullable: false),
                    Page = table.Column<int>(type: "integer", nullable: false),
                    QrCodeData = table.Column<string>(type: "character(32)", fixedLength: true, maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamBookletPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamBookletPage_ExamBooklet_BookletId",
                        column: x => x.BookletId,
                        principalTable: "ExamBooklet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamBooklet_ExamId",
                table: "ExamBooklet",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBookletPage_BookletId",
                table: "ExamBookletPage",
                column: "BookletId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBookletPage_QrCodeData",
                table: "ExamBookletPage",
                column: "QrCodeData",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_File_ExamBooklet_BookletId",
                table: "File",
                column: "BookletId",
                principalTable: "ExamBooklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submission_ExamBooklet_BookletId",
                table: "Submission",
                column: "BookletId",
                principalTable: "ExamBooklet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionPage_ExamBookletPage_BookletPageId",
                table: "SubmissionPage",
                column: "BookletPageId",
                principalTable: "ExamBookletPage",
                principalColumn: "Id");
        }
    }
}
