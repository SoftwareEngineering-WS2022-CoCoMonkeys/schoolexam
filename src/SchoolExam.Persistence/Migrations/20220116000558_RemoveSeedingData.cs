using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class RemoveSeedingData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                table: "ExamCourse",
                keyColumns: new[] { "ExamId", "ParticipantId" },
                keyValues: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"));

            migrationBuilder.DeleteData(
                table: "School",
                keyColumn: "Id",
                keyValue: new Guid("04bceee7-a744-48a7-9a0a-eda2d4a142d5"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("16771069-c615-4e02-8703-0ff100d1b0b7"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("314ddd2e-62bb-4a29-8517-bb38ef96c897"));

            migrationBuilder.DeleteData(
                table: "Course",
                keyColumn: "Id",
                keyValue: new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"));

            migrationBuilder.DeleteData(
                table: "ExamParticipant",
                keyColumns: new[] { "ExamId", "ParticipantId" },
                keyValues: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("c0242654-af32-4115-abea-c9814a8f91bb"));

            migrationBuilder.DeleteData(
                table: "Exam",
                keyColumn: "Id",
                keyValue: new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Course",
                columns: new[] { "Id", "Name", "SchoolId", "Topic" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), "Sozialwissenschaften 2022", new Guid("04bceee7-a744-48a7-9a0a-eda2d4a142d5"), "Sozialwissenschaften" });

            migrationBuilder.InsertData(
                table: "Exam",
                columns: new[] { "Id", "CreatorId", "Date", "DueDate", "State", "Title", "Topic" },
                values: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("c0242654-af32-4115-abea-c9814a8f91bb"), new DateTime(2022, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2022, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), 0, "1. Schulaufgabe", "Sozialwissenschaften" });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "DateOfBirth", "Discriminator", "EmailAddress", "FirstName", "LastName", "City", "Country", "PostalCode", "StreetName", "StreetNumber" },
                values: new object[,]
                {
                    { new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"), new DateTime(2004, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Student", "amira.jabbar@school-exam.de", "Amira", "Jabbar", "München", "Deutschland", "80333", "You-Go-Girl-Allee", "99" },
                    { new Guid("c0242654-af32-4115-abea-c9814a8f91bb"), new DateTime(1974, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Teacher", "thorsten.thurn@school-exam.de", "Briggite", "Schweinebauer", "Hamburg", "Deutschland", "20095", "Klarer-Kopf-Weg", "1a" }
                });

            migrationBuilder.InsertData(
                table: "School",
                columns: new[] { "Id", "Name", "City", "Country", "PostalCode", "StreetName", "StreetNumber" },
                values: new object[] { new Guid("04bceee7-a744-48a7-9a0a-eda2d4a142d5"), "Schmuttertal-Gymnasium Diedorf", "Diedorf", "Deutschland", "86420", "Schmetterlingsplatz", "1" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Password", "PersonId", "Username", "Role" },
                values: new object[] { new Guid("16771069-c615-4e02-8703-0ff100d1b0b7"), "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2", null, "admin2", "Administrator" });

            migrationBuilder.InsertData(
                table: "CourseStudent",
                columns: new[] { "CourseId", "StudentId" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894") });

            migrationBuilder.InsertData(
                table: "CourseTeacher",
                columns: new[] { "CourseId", "TeacherId" },
                values: new object[] { new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a"), new Guid("c0242654-af32-4115-abea-c9814a8f91bb") });

            migrationBuilder.InsertData(
                table: "ExamParticipant",
                columns: new[] { "ExamId", "ParticipantId" },
                values: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Password", "PersonId", "Username", "Role" },
                values: new object[] { new Guid("314ddd2e-62bb-4a29-8517-bb38ef96c897"), "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2", new Guid("c0242654-af32-4115-abea-c9814a8f91bb"), "admin", "Teacher" });

            migrationBuilder.InsertData(
                table: "ExamCourse",
                columns: new[] { "ExamId", "ParticipantId" },
                values: new object[] { new Guid("4c9be4e7-5507-46b2-9b9e-9746c931ee25"), new Guid("e5fa7d18-dddd-4969-b22a-12f89ac0b18a") });
        }
    }
}
