using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddTestExam : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "Id", "Description", "Name", "SchoolId", "Subject" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), "Projektmanagement, etc.", "Sozialwissenschaften 2022", new Guid("04bceee7-a744-48a7-9a0a-eda2d4a142d5"), "Sozialwissenschaften" });

            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "DateOfBirth", "Discriminator", "EmailAddress", "FirstName", "LastName", "City", "Country", "PostalCode", "StreetName", "StreetNumber" },
                values: new object[] { new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"), new DateTime(2004, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Student", "amira.jabbar@school-exam.de", "Amira", "Jabbar", "München", "Deutschland", "80333", "You-Go-Girl-Allee", "99" });

            migrationBuilder.InsertData(
                table: "CourseStudent",
                columns: new[] { "CourseId", "StudentId" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894") });

            migrationBuilder.InsertData(
                table: "CourseTeacher",
                columns: new[] { "CourseId", "TeacherId" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("c0242654-af32-4115-abea-c9814a8f91bb") });

            migrationBuilder.InsertData(
                table: "Exam",
                columns: new[] { "Id", "CourseId", "CreatorId", "Date", "Description", "DueDate", "GradingTableId", "State", "Title" },
                values: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("c0242654-af32-4115-abea-c9814a8f91bb"), new DateTime(2022, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mündliche Leistungsfeststellung", new DateTime(2022, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, 0, "Projektmanagement" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CourseStudent",
                keyColumns: new[] { "CourseId", "StudentId" },
                keyValues: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894") });

            migrationBuilder.DeleteData(
                table: "CourseTeacher",
                keyColumns: new[] { "CourseId", "TeacherId" },
                keyValues: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("c0242654-af32-4115-abea-c9814a8f91bb") });

            migrationBuilder.DeleteData(
                table: "Exam",
                keyColumn: "Id",
                keyValue: new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"));

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"));

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "Id",
                keyValue: new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"));
        }
    }
}
