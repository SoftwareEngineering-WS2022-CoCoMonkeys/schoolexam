using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddTestTeacher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Person",
                columns: new[] { "Id", "DateOfBirth", "Discriminator", "EmailAddress", "FirstName", "LastName", "City", "Country", "PostalCode", "StreetName", "StreetNumber" },
                values: new object[] { new Guid("c0242654-af32-4115-abea-c9814a8f91bb"), new DateTime(1974, 5, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Teacher", "thorsten.thurn@school-exam.de", "Briggite", "Schweinebauer", "Hamburg", "Deutschland", "20095", "Klarer-Kopf-Weg", "1a" });

            migrationBuilder.InsertData(
                table: "School",
                columns: new[] { "Id", "Name", "City", "Country", "PostalCode", "StreetName", "StreetNumber" },
                values: new object[] { new Guid("04bceee7-a744-48a7-9a0a-eda2d4a142d5"), "Schmuttertal-Gymnasium Diedorf", "Diedorf", "Deutschland", "86420", "Schmetterlingsplatz", "1" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Password", "PersonId", "Username", "Role" },
                values: new object[] { new Guid("314ddd2e-62bb-4a29-8517-bb38ef96c897"), "$2a$11$3Q8Re.PhjBIPqPIqzAy3Y./XFRjcelEOr7kL0X27ljVbay1PwTMw2", new Guid("c0242654-af32-4115-abea-c9814a8f91bb"), "admin", "Teacher" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "School",
                keyColumn: "Id",
                keyValue: new Guid("04bceee7-a744-48a7-9a0a-eda2d4a142d5"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: new Guid("314ddd2e-62bb-4a29-8517-bb38ef96c897"));

            migrationBuilder.DeleteData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("c0242654-af32-4115-abea-c9814a8f91bb"));
        }
    }
}
