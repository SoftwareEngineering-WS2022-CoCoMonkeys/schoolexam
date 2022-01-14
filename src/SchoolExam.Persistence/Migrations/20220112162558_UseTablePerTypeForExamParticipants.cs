using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class UseTablePerTypeForExamParticipants : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamParticipant_Course_ParticipantId",
                table: "ExamParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamParticipant_Person_ParticipantId",
                table: "ExamParticipant");

            migrationBuilder.DropIndex(
                name: "IX_ExamParticipant_ParticipantId",
                table: "ExamParticipant");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "ExamParticipant");

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

            migrationBuilder.InsertData(
                table: "ExamParticipant",
                columns: new[] { "ExamId", "ParticipantId" },
                values: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.InsertData(
                table: "ExamCourse",
                columns: new[] { "ExamId", "ParticipantId" },
                values: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });

            migrationBuilder.CreateIndex(
                name: "IX_ExamCourse_ParticipantId",
                table: "ExamCourse",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStudent_ParticipantId",
                table: "ExamStudent",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamCourse");

            migrationBuilder.DropTable(
                name: "ExamStudent");

            migrationBuilder.DeleteData(
                table: "ExamParticipant",
                keyColumns: new[] { "ExamId", "ParticipantId" },
                keyValues: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "ExamParticipant",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_ExamParticipant_ParticipantId",
                table: "ExamParticipant",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamParticipant_Course_ParticipantId",
                table: "ExamParticipant",
                column: "ParticipantId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamParticipant_Person_ParticipantId",
                table: "ExamParticipant",
                column: "ParticipantId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
