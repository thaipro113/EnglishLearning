using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearning.Migrations
{
    /// <inheritdoc />
    public partial class FixMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop ChatHistories if exists
            migrationBuilder.Sql("IF OBJECT_ID('ChatHistories', 'U') IS NOT NULL DROP TABLE ChatHistories");

            // Create FlashcardSets if not exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('FlashcardSets', 'U') IS NULL
                BEGIN
                    CREATE TABLE [FlashcardSets] (
                        [FlashcardSetId] int NOT NULL IDENTITY,
                        [Title] nvarchar(max) NOT NULL,
                        [Description] nvarchar(max) NOT NULL,
                        [CreatedAt] datetime2 NOT NULL,
                        CONSTRAINT [PK_FlashcardSets] PRIMARY KEY ([FlashcardSetId])
                    );
                END
            ");

            // Create Flashcards if not exists
            migrationBuilder.Sql(@"
                IF OBJECT_ID('Flashcards', 'U') IS NULL
                BEGIN
                    CREATE TABLE [Flashcards] (
                        [FlashcardId] int NOT NULL IDENTITY,
                        [FlashcardSetId] int NOT NULL,
                        [FrontText] nvarchar(max) NOT NULL,
                        [BackText] nvarchar(max) NOT NULL,
                        CONSTRAINT [PK_Flashcards] PRIMARY KEY ([FlashcardId]),
                        CONSTRAINT [FK_Flashcards_FlashcardSets_FlashcardSetId] FOREIGN KEY ([FlashcardSetId]) REFERENCES [FlashcardSets] ([FlashcardSetId]) ON DELETE CASCADE
                    );
                END
            ");

            // Create Index if not exists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name='IX_Flashcards_FlashcardSetId' AND object_id = OBJECT_ID('Flashcards'))
                BEGIN
                    CREATE INDEX [IX_Flashcards_FlashcardSetId] ON [Flashcards] ([FlashcardSetId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.DropTable(
                name: "Flashcards");

            migrationBuilder.DropTable(
                name: "FlashcardSets");

            // Recreate ChatHistories (simplified, assumes user wants to revert)
              migrationBuilder.CreateTable(
                name: "ChatHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatHistories_UserId",
                table: "ChatHistories",
                column: "UserId");
        }
    }
}
