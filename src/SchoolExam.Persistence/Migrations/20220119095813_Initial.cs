using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Topic = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
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
                    Country = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_School", x => x.Id);
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
                    EmailAddress = table.Column<string>(type: "text", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false),
                    QrCodeData = table.Column<string>(type: "text", nullable: true),
                    SchoolId = table.Column<Guid>(type: "uuid", nullable: true),
                    Teacher_SchoolId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Person_School_Teacher_SchoolId",
                        column: x => x.Teacher_SchoolId,
                        principalTable: "School",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    Topic = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exam_Person_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentLegalGuardian",
                columns: table => new
                {
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LegalGuardianId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLegalGuardian", x => new { x.StudentId, x.LegalGuardianId });
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
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    PersonId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id");
                });

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
                name: "ExamParticipant",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamParticipant", x => new { x.ExamId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_ExamParticipant_Exam_ExamId",
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
                    MaxPoints = table.Column<double>(type: "double precision", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    StartPage = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<double>(type: "double precision", nullable: false),
                    EndPage = table.Column<int>(type: "integer", nullable: false),
                    EndY = table.Column<double>(type: "double precision", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTask_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradingTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingTable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradingTable_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledExam",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    PublishTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsPublished = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledExam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduledExam_Exam_ExamId",
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
                    QrCodeData = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookletId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submission_Booklet_BookletId",
                        column: x => x.BookletId,
                        principalTable: "Booklet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submission_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExamCourse",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCourse", x => new { x.ExamId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_ExamCourse_Course_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamCourse_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamCourse_ExamParticipant_ExamId_ParticipantId",
                        columns: x => new { x.ExamId, x.ParticipantId },
                        principalTable: "ExamParticipant",
                        principalColumns: new[] { "ExamId", "ParticipantId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamStudent",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamStudent", x => new { x.ExamId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_ExamStudent_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamStudent_ExamParticipant_ExamId_ParticipantId",
                        columns: x => new { x.ExamId, x.ParticipantId },
                        principalTable: "ExamParticipant",
                        principalColumns: new[] { "ExamId", "ParticipantId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamStudent_Person_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradingTableInterval",
                columns: table => new
                {
                    GradingTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartPoints = table.Column<double>(type: "double precision", nullable: false),
                    StartType = table.Column<int>(type: "integer", nullable: false),
                    EndPoints = table.Column<double>(type: "double precision", nullable: false),
                    EndType = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingTableInterval", x => new { x.GradingTableId, x.Id });
                    table.ForeignKey(
                        name: "FK_GradingTableInterval_GradingTable_GradingTableId",
                        column: x => x.GradingTableId,
                        principalTable: "GradingTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Answer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    AchievedPoints = table.Column<double>(type: "double precision", nullable: true),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Answer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Answer_ExamTask_TaskId",
                        column: x => x.TaskId,
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
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookletPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    QrCodeData = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionPage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionPage_BookletPage_BookletPageId",
                        column: x => x.BookletPageId,
                        principalTable: "BookletPage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubmissionPage_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionPage_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AnswerSegment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StartPage = table.Column<int>(type: "integer", nullable: false),
                    StartY = table.Column<double>(type: "double precision", nullable: false),
                    EndPage = table.Column<int>(type: "integer", nullable: false),
                    EndY = table.Column<double>(type: "double precision", nullable: false),
                    AnswerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerSegment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerSegment_Answer_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "Answer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                    SubmissionId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmissionPageId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmissionPdfFile_SubmissionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_Booklet_BookletId",
                        column: x => x.BookletId,
                        principalTable: "Booklet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_Submission_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "Submission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_File_Submission_SubmissionPdfFile_SubmissionId",
                        column: x => x.SubmissionPdfFile_SubmissionId,
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
                name: "IX_Answer_SubmissionId",
                table: "Answer",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Answer_TaskId",
                table: "Answer",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSegment_AnswerId",
                table: "AnswerSegment",
                column: "AnswerId");

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

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudent_StudentId",
                table: "CourseStudent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeacher_TeacherId",
                table: "CourseTeacher",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CreatorId",
                table: "Exam",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamCourse_ParticipantId",
                table: "ExamCourse",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStudent_ParticipantId",
                table: "ExamStudent",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTask_ExamId",
                table: "ExamTask",
                column: "ExamId");

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
                name: "IX_File_SubmissionPdfFile_SubmissionId",
                table: "File",
                column: "SubmissionPdfFile_SubmissionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_UploaderId",
                table: "File",
                column: "UploaderId");

            migrationBuilder.CreateIndex(
                name: "IX_GradingTable_ExamId",
                table: "GradingTable",
                column: "ExamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_QrCodeData",
                table: "Person",
                column: "QrCodeData",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Person_Teacher_SchoolId",
                table: "Person",
                column: "Teacher_SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledExam_ExamId",
                table: "ScheduledExam",
                column: "ExamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLegalGuardian_LegalGuardianId",
                table: "StudentLegalGuardian",
                column: "LegalGuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Submission_BookletId",
                table: "Submission",
                column: "BookletId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Submission_StudentId",
                table: "Submission",
                column: "StudentId");

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
                name: "IX_SubmissionPage_QrCodeData",
                table: "SubmissionPage",
                column: "QrCodeData");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionPage_SubmissionId",
                table: "SubmissionPage",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_User_PersonId",
                table: "User",
                column: "PersonId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Username",
                table: "User",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnswerSegment");

            migrationBuilder.DropTable(
                name: "CourseStudent");

            migrationBuilder.DropTable(
                name: "CourseTeacher");

            migrationBuilder.DropTable(
                name: "ExamCourse");

            migrationBuilder.DropTable(
                name: "ExamStudent");

            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "GradingTableInterval");

            migrationBuilder.DropTable(
                name: "ScheduledExam");

            migrationBuilder.DropTable(
                name: "StudentLegalGuardian");

            migrationBuilder.DropTable(
                name: "Answer");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "ExamParticipant");

            migrationBuilder.DropTable(
                name: "SubmissionPage");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "GradingTable");

            migrationBuilder.DropTable(
                name: "ExamTask");

            migrationBuilder.DropTable(
                name: "BookletPage");

            migrationBuilder.DropTable(
                name: "Submission");

            migrationBuilder.DropTable(
                name: "Booklet");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "School");
        }
    }
}
