using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnglishLearning.Migrations
{
    /// <inheritdoc />
    public partial class InitialDockerCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlashcardSets",
                columns: table => new
                {
                    FlashcardSetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardSets", x => x.FlashcardSetId);
                });

            migrationBuilder.CreateTable(
                name: "LearningLevels",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Learning__09F03C26EC2A3FB4", x => x.LevelId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "User"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CC4CC30BF25F", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Flashcards",
                columns: table => new
                {
                    FlashcardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlashcardSetId = table.Column<int>(type: "int", nullable: false),
                    FrontText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BackText = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flashcards", x => x.FlashcardId);
                    table.ForeignKey(
                        name: "FK_Flashcards_FlashcardSets_FlashcardSetId",
                        column: x => x.FlashcardSetId,
                        principalTable: "FlashcardSets",
                        principalColumn: "FlashcardSetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubLevels",
                columns: table => new
                {
                    SubLevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubLevelName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LevelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubLevels", x => x.SubLevelId);
                    table.ForeignKey(
                        name: "FK_SubLevels_LearningLevels_LevelId",
                        column: x => x.LevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "LevelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Paragraphs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParagraphText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paragraphs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paragraphs_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubLevelId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Courses__C92D71A705F7D0AF", x => x.CourseId);
                    table.ForeignKey(
                        name: "FK_Courses_SubLevelId",
                        column: x => x.SubLevelId,
                        principalTable: "SubLevels",
                        principalColumn: "SubLevelId");
                });

            migrationBuilder.CreateTable(
                name: "TranslationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ParagraphId = table.Column<int>(type: "int", nullable: false),
                    Topic = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalSentence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserTranslation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StandardTranslation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accuracy = table.Column<double>(type: "float", nullable: false),
                    ErrorSuggestion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationHistories_ParagraphId",
                        column: x => x.ParagraphId,
                        principalTable: "Paragraphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TranslationHistories_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    LessonId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(10)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    LessonType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AudioPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Lessons__B084ACD0F7EAA506", x => x.LessonId);
                    table.ForeignKey(
                        name: "FK__Lessons__CourseI__45F365D3",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Progresses",
                columns: table => new
                {
                    ProgressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LessonId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Score = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Progress__BAE29CA5B14894C3", x => x.ProgressId);
                    table.ForeignKey(
                        name: "FK__Progresse__Lesso__4AB81AF0",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Progresse__UserI__49C3F6B7",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LessonId = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    Question = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ImageGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                    table.ForeignKey(
                        name: "FK_Quizzes_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SubLevelId",
                table: "Courses",
                column: "SubLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_FlashcardSetId",
                table: "Flashcards",
                column: "FlashcardSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Paragraphs_UserId",
                table: "Paragraphs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_LessonId",
                table: "Progresses",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Progresses_UserId",
                table: "Progresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_LessonId",
                table: "Quizzes",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_SubLevels_LevelId",
                table: "SubLevels",
                column: "LevelId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationHistories_ParagraphId",
                table: "TranslationHistories",
                column: "ParagraphId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationHistories_UserId",
                table: "TranslationHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flashcards");

            migrationBuilder.DropTable(
                name: "Progresses");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "TranslationHistories");

            migrationBuilder.DropTable(
                name: "FlashcardSets");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Paragraphs");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SubLevels");

            migrationBuilder.DropTable(
                name: "LearningLevels");
        }
    }
}
