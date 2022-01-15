using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddsAdmin2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Exam");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Course");

            migrationBuilder.UpdateData(
                table: "Exam",
                keyColumn: "Id",
                keyValue: new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"),
                column: "Title",
                value: "1. Schulaufgabe");

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Password", "PersonId", "Username", "Role" },
                values: new object[] { new Guid("16771069-c615-4e02-8703-0ff100d1b0b7"), "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2", null, "admin2", "Administrator" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("16771069-c615-4e02-8703-0ff100d1b0b7"));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Exam",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Course",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Course",
                keyColumn: "Id",
                keyValue: new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"),
                column: "Description",
                value: "Projektmanagement, etc.");

            migrationBuilder.UpdateData(
                table: "Exam",
                keyColumn: "Id",
                keyValue: new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"),
                columns: new[] { "Description", "Title" },
                values: new object[] { "", "" });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");
        }
    }
}
