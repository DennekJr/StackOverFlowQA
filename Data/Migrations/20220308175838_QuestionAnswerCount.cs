using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackOverFlowQA.Data.Migrations
{
    public partial class QuestionAnswerCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnswerCount",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnswerCount",
                table: "Questions");
        }
    }
}
