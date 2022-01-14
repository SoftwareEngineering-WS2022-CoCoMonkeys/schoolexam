using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddAnswerSegment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_ExamTask_ExamTaskId",
                table: "Answer");

            migrationBuilder.DropForeignKey(
                name: "FK_Exam_Course_CourseId",
                table: "Exam");

            migrationBuilder.DropIndex(
                name: "IX_Exam_CourseId",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Exam");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "ExamTask",
                newName: "StartY");

            migrationBuilder.RenameColumn(
                name: "Page",
                table: "ExamTask",
                newName: "StartPage");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "ExamTask",
                newName: "EndPage");

            migrationBuilder.RenameColumn(
                name: "ExamTaskId",
                table: "Answer",
                newName: "TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_ExamTaskId",
                table: "Answer",
                newName: "IX_Answer_TaskId");

            migrationBuilder.AddColumn<double>(
                name: "EndY",
                table: "ExamTask",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "AchievedPoints",
                table: "Answer",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Answer",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerSegment_AnswerId",
                table: "AnswerSegment",
                column: "AnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_ExamTask_TaskId",
                table: "Answer",
                column: "TaskId",
                principalTable: "ExamTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_ExamTask_TaskId",
                table: "Answer");

            migrationBuilder.DropTable(
                name: "AnswerSegment");

            migrationBuilder.DropColumn(
                name: "EndY",
                table: "ExamTask");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Answer");

            migrationBuilder.RenameColumn(
                name: "StartY",
                table: "ExamTask",
                newName: "Y");

            migrationBuilder.RenameColumn(
                name: "StartPage",
                table: "ExamTask",
                newName: "Page");

            migrationBuilder.RenameColumn(
                name: "EndPage",
                table: "ExamTask",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                table: "Answer",
                newName: "ExamTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_Answer_TaskId",
                table: "Answer",
                newName: "IX_Answer_ExamTaskId");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseId",
                table: "Exam",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "AchievedPoints",
                table: "Answer",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_CourseId",
                table: "Exam",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_ExamTask_ExamTaskId",
                table: "Answer",
                column: "ExamTaskId",
                principalTable: "ExamTask",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_Course_CourseId",
                table: "Exam",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");
        }
    }
}
