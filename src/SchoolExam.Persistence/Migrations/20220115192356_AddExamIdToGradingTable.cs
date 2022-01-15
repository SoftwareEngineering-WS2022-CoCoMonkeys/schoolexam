using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddExamIdToGradingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exam_GradingTable_GradingTableId",
                table: "Exam");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamTask_Exam_ExamId",
                table: "ExamTask");

            migrationBuilder.DropIndex(
                name: "IX_Exam_GradingTableId",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "GradingTableId",
                table: "Exam");

            migrationBuilder.AlterColumn<double>(
                name: "StartPoints",
                table: "GradingTableInterval",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Grade",
                table: "GradingTableInterval",
                type: "text",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "EndPoints",
                table: "GradingTableInterval",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "ExamId",
                table: "GradingTable",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamId",
                table: "ExamTask",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_GradingTable_ExamId",
                table: "GradingTable",
                column: "ExamId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamTask_Exam_ExamId",
                table: "ExamTask",
                column: "ExamId",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GradingTable_Exam_ExamId",
                table: "GradingTable",
                column: "ExamId",
                principalTable: "Exam",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamTask_Exam_ExamId",
                table: "ExamTask");

            migrationBuilder.DropForeignKey(
                name: "FK_GradingTable_Exam_ExamId",
                table: "GradingTable");

            migrationBuilder.DropIndex(
                name: "IX_GradingTable_ExamId",
                table: "GradingTable");

            migrationBuilder.DropColumn(
                name: "ExamId",
                table: "GradingTable");

            migrationBuilder.AlterColumn<int>(
                name: "StartPoints",
                table: "GradingTableInterval",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<double>(
                name: "Grade",
                table: "GradingTableInterval",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "EndPoints",
                table: "GradingTableInterval",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamId",
                table: "ExamTask",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "GradingTableId",
                table: "Exam",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_GradingTableId",
                table: "Exam",
                column: "GradingTableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exam_GradingTable_GradingTableId",
                table: "Exam",
                column: "GradingTableId",
                principalTable: "GradingTable",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamTask_Exam_ExamId",
                table: "ExamTask",
                column: "ExamId",
                principalTable: "Exam",
                principalColumn: "Id");
        }
    }
}
