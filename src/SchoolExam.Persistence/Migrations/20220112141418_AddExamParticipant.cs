using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddExamParticipant : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_Course_CourseId",
                table: "Exam");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Course",
                newName: "Topic");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Exam",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "Exam",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ExamParticipant",
                columns: table => new
                {
                    ExamId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uuid", nullable: false),
                    Discriminator = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamParticipant", x => new { x.ExamId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_ExamParticipant_Course_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Course",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamParticipant_Exam_ExamId",
                        column: x => x.ExamId,
                        principalTable: "Exam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamParticipant_Person_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Exam",
                keyColumn: "Id",
                keyValue: new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"),
                columns: new[] { "CourseId", "Description", "Title", "Topic" },
                values: new object[] { null, "", "", "Sozialwissenschaften" });

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
                name: "FK_Exam_Course_CourseId",
                table: "Exam",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_Course_CourseId",
                table: "Exam");

            migrationBuilder.DropTable(
                name: "ExamParticipant");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "Exam");

            migrationBuilder.RenameColumn(
                name: "Topic",
                table: "Course",
                newName: "Subject");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Exam",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Exam",
                keyColumn: "Id",
                keyValue: new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"),
                columns: new[] { "CourseId", "Description", "Title" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), "Mündliche Leistungsfeststellung", "Projektmanagement" });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_Course_CourseId",
                table: "Exam",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
