using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class IntroduceExplicitManyToManyEntitiesAndRemoveTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_Person_TeacherId",
                table: "Course");

            migrationBuilder.DropTable(
                name: "Remark");

            migrationBuilder.DropTable(
                name: "Task");

            migrationBuilder.DropTable(
                name: "Input");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLegalGuardian",
                table: "StudentLegalGuardian");

            migrationBuilder.DropIndex(
                name: "IX_StudentLegalGuardian_StudentId",
                table: "StudentLegalGuardian");

            migrationBuilder.DropIndex(
                name: "IX_Course_TeacherId",
                table: "Course");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("640b0338-f0d0-4033-9c13-9f021417cae7"));

            migrationBuilder.DropColumn(
                name: "AchievedPoints",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "IsDigital",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Submission");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "ExamTask");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Answer");

            migrationBuilder.AddColumn<Guid>(
                name: "Teacher_SchoolId",
                table: "Person",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Exam",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<double>(
                name: "AchievedPoints",
                table: "Answer",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLegalGuardian",
                table: "StudentLegalGuardian",
                columns: new[] { "StudentId", "LegalGuardianId" });

            migrationBuilder.CreateTable(
                name: "CourseTeacher",
                columns: table => new
                {
                    CourseId = table.Column<Guid>(type: "uuid", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTeacher", x => new { x.CourseId, x.TeacherId });
                    table.ForeignKey(
                        name: "FK_CourseTeacher_Course_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTeacher_Person_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLegalGuardian_LegalGuardianId",
                table: "StudentLegalGuardian",
                column: "LegalGuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CreatorId",
                table: "Exam",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeacher_TeacherId",
                table: "CourseTeacher",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_Person_CreatorId",
                table: "Exam",
                column: "CreatorId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_Person_CreatorId",
                table: "Exam");

            migrationBuilder.DropTable(
                name: "CourseTeacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentLegalGuardian",
                table: "StudentLegalGuardian");

            migrationBuilder.DropIndex(
                name: "IX_StudentLegalGuardian_LegalGuardianId",
                table: "StudentLegalGuardian");

            migrationBuilder.DropIndex(
                name: "IX_Exam_CreatorId",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "Teacher_SchoolId",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Exam");

            migrationBuilder.AddColumn<int>(
                name: "AchievedPoints",
                table: "Submission",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDigital",
                table: "Submission",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Submission",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "ExamTask",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Course",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AchievedPoints",
                table: "Answer",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Answer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentLegalGuardian",
                table: "StudentLegalGuardian",
                columns: new[] { "LegalGuardianId", "StudentId" });

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
                name: "Task",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Task", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Remark",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InputId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Password", "PersonId", "Role", "Username" },
                values: new object[] { new Guid("640b0338-f0d0-4033-9c13-9f021417cae7"), "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2", null, "Administrator", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentLegalGuardian_StudentId",
                table: "StudentLegalGuardian",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TeacherId",
                table: "Course",
                column: "TeacherId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Person_TeacherId",
                table: "Course",
                column: "TeacherId",
                principalTable: "Person",
                principalColumn: "Id");
        }
    }
}
