using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchoolExam.Infrastructure.Migrations
{
    public partial class AddGradingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Subject = table.Column<string>(type: "text", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false, defaultValue: 2021)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GradingTable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TempId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingTable", x => x.Id);
                    table.UniqueConstraint("AK_GradingTable_TempId", x => x.TempId);
                });

            migrationBuilder.CreateTable(
                name: "GradingTableInterval",
                columns: table => new
                {
                    GradingTableId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartPoints = table.Column<int>(type: "integer", nullable: false),
                    StartType = table.Column<int>(type: "integer", nullable: false),
                    EndPoints = table.Column<int>(type: "integer", nullable: false),
                    EndType = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<double>(type: "double precision", nullable: false)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "GradingTableInterval");

            migrationBuilder.DropTable(
                name: "GradingTable");
        }
    }
}
