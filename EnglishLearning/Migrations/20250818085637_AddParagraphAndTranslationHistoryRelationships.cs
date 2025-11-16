using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearning.Migrations
{
    /// <inheritdoc />
    public partial class AddParagraphAndTranslationHistoryRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TranslationHistories_ParagraphId",
                table: "TranslationHistories",
                column: "ParagraphId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationHistories_UserId",
                table: "TranslationHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Paragraphs_UserId",
                table: "Paragraphs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paragraphs_UserId",
                table: "Paragraphs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationHistories_ParagraphId",
                table: "TranslationHistories",
                column: "ParagraphId",
                principalTable: "Paragraphs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationHistories_UserId",
                table: "TranslationHistories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paragraphs_UserId",
                table: "Paragraphs");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationHistories_ParagraphId",
                table: "TranslationHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationHistories_UserId",
                table: "TranslationHistories");

            migrationBuilder.DropIndex(
                name: "IX_TranslationHistories_ParagraphId",
                table: "TranslationHistories");

            migrationBuilder.DropIndex(
                name: "IX_TranslationHistories_UserId",
                table: "TranslationHistories");

            migrationBuilder.DropIndex(
                name: "IX_Paragraphs_UserId",
                table: "Paragraphs");
        }
    }
}
