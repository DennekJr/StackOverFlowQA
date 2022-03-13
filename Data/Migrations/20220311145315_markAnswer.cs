using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackOverFlowQA.Data.Migrations
{
    public partial class markAnswer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswerCount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AnswerIsCorrent",
                table: "Answers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswerCount",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AnswerIsCorrent",
                table: "Answers");
        }
    }
}
