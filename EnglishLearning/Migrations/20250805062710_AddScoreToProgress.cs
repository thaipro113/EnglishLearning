using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreToProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Score",
                table: "Progresses",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Progresses");
        }
    }
}
