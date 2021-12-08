using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_GradingTable_TempId",
                table: "GradingTable");

            migrationBuilder.DropColumn(
                name: "TempId",
                table: "GradingTable");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Course",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Course",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "SchoolId",
                table: "Course",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Course",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GradingTableId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exam_GradingTable_GradingTableId",
                        column: x => x.GradingTableId,
                        principalTable: "GradingTable",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Input",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Input", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StreetName = table.Column<string>(type: "text", nullable: true),
                    StreetNumber = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "School",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    StreetName = table.Column<string>(type: "text", nullable: false),
                    StreetNumber = table.Column<string>(type: "text", nullable: false),
                    PostalCode = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    HasScanner = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExamBooklet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false)
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
                name: "ExamTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    MaxPoints = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    StartPage = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<double>(type: "double precision", nullable: false),
                    EndPage = table.Column<int>(type: "integer", nullable: false),
                    EndY = table.Column<double>(type: "double precision", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTask_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CourseStudent",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudent", x => new { x.CourseId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_CourseStudent_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudent_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentLegalGuardian",
                columns: table => new
                {
                    LegalGuardianId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLegalGuardian", x => new { x.LegalGuardianId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_StudentLegalGuardian_Person_LegalGuardianId",
                        column: x => x.LegalGuardianId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentLegalGuardian_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchoolTeacher",
                columns: table => new
                {
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTeacher", x => new { x.SchoolId, x.TeacherId });
                    table.ForeignKey(
                        name: "FK_SchoolTeacher_Person_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchoolTeacher_School_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamBookletPage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Page = table.Column<int>(type: "integer", nullable: false),
                    ExamBookletId = table.Column<Guid>(type: "uuid", nullable: false),
                    QrCode = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamBookletPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamBookletPage_ExamBooklet_ExamBookletId",
                        column: x => x.ExamBookletId,
                        principalTable: "ExamBooklet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDigital = table.Column<bool>(type: "boolean", nullable: false),
                    BookletId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamBookletId = table.Column<Guid>(type: "uuid", nullable: true),
                    State = table.Column<int>(type: "integer", nullable: false),
                    AchievedPoints = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submission_ExamBooklet_ExamBookletId",
                        column: x => x.ExamBookletId,
                        principalTable: "ExamBooklet",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Submission_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    AchievedPoints = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answer_ExamTask_ExamTaskId",
                        column: x => x.ExamTaskId,
                        principalTable: "ExamTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Answer_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionPage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Page = table.Column<int>(type: "integer", nullable: false),
                    ScanData = table.Column<byte[]>(type: "bytea", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionPage_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Remark",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InputId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remark", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Remark_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Remark_Input_InputId",
                        column: x => x.InputId,
                        principalTable: "Input",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Remark_Person_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Remark_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_TeacherId",
                table: "Course",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_ExamTaskId",
                table: "Answer",
                column: "ExamTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_SubmissionId",
                table: "Answer",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudent_StudentId",
                table: "CourseStudent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CourseId",
                table: "Exam",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_GradingTableId",
                table: "Exam",
                column: "GradingTableId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBooklet_ExamId",
                table: "ExamBooklet",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBookletPage_ExamBookletId",
                table: "ExamBookletPage",
                column: "ExamBookletId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBookletPage_QrCode",
                table: "ExamBookletPage",
                column: "QrCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamTask_ExamId",
                table: "ExamTask",
                column: "ExamId");

            migrationBuilder.CreateIndex(
                name: "IX_Remark_AnswerId",
                table: "Remark",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_Remark_InputId",
                table: "Remark",
                column: "InputId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Remark_SubmissionId",
                table: "Remark",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Remark_TeacherId",
                table: "Remark",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolTeacher_TeacherId",
                table: "SchoolTeacher",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentLegalGuardian_StudentId",
                table: "StudentLegalGuardian",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_ExamBookletId",
                table: "Submission",
                column: "ExamBookletId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_StudentId",
                table: "Submission",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_SubmissionId",
                table: "SubmissionPage",
                column: "SubmissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Person_TeacherId",
                table: "Course",
                column: "TeacherId",
                principalTable: "Person",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Person_TeacherId",
                table: "Course");

            migrationBuilder.DropTable(
                name: "CourseStudent");

            migrationBuilder.DropTable(
                name: "ExamBookletPage");

            migrationBuilder.DropTable(
                name: "Remark");

            migrationBuilder.DropTable(
                name: "SchoolTeacher");

            migrationBuilder.DropTable(
                name: "StudentLegalGuardian");

            migrationBuilder.DropTable(
                name: "SubmissionPage");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "Input");

            migrationBuilder.DropTable(
                name: "School");

            migrationBuilder.DropTable(
                name: "ExamTask");

            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.DropTable(
                name: "ExamBooklet");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropIndex(
                name: "IX_Course_TeacherId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Course");

            migrationBuilder.AddColumn<int>(
                name: "TempId",
                table: "GradingTable",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_GradingTable_TempId",
                table: "GradingTable",
                column: "TempId");
        }
    }
}
