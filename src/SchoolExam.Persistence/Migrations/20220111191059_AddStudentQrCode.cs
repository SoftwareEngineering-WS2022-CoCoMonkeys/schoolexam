using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolExam.Persistence.Migrations
{
    public partial class AddStudentQrCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QrCodeData",
                table: "Person",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "QrCodeData",
                table: "BookletPage",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(32)",
                oldFixedLength: true,
                oldMaxLength: 32);

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: new Guid("3e0fe3ab-3a84-43b1-a501-11ffb47fc894"),
                column: "QrCodeData",
                value: "d18b19227701139f25eb4f205f785995");

            migrationBuilder.CreateIndex(
                name: "IX_Person_QrCodeData",
                table: "Person",
                column: "QrCodeData",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Person_QrCodeData",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "QrCodeData",
                table: "Person");

            migrationBuilder.AlterColumn<string>(
                name: "QrCodeData",
                table: "BookletPage",
                type: "character(32)",
                fixedLength: true,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
