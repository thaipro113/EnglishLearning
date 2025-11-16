using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EnglishLearning.Models
{
    public partial class EnglishLearningDbContext : DbContext
    {
        public EnglishLearningDbContext()
        {
        }

        public EnglishLearningDbContext(DbContextOptions<EnglishLearningDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<LearningLevel> LearningLevels { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Progress> Progresses { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<SubLevel> SubLevels { get; set; }
        public DbSet<ChatHistory> ChatHistories { get; set; }
        public DbSet<TranslationHistory> TranslationHistories { get; set; }
        public DbSet<Paragraph> Paragraphs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#pragma warning disable CS8604 // Possible null reference argument.
            => optionsBuilder.UseSqlServer("Data Source=thaipro113\\SQLEXPRESS;Initial Catalog=EnglishLearningDB;Integrated Security=True;Trust Server Certificate=True");
#pragma warning restore CS8604 // Possible null reference argument.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.CourseId).HasName("PK__Courses__C92D71A705F7D0AF");
                entity.Property(e => e.CourseId).HasMaxLength(10); // Định nghĩa CourseId là string, tự nhập
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.HasOne(d => d.SubLevel) // Thay Level bằng SubLevel
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.SubLevelId) // Sử dụng SubLevelId làm khóa ngoại
                    .HasConstraintName("FK_Courses_SubLevelId")
                    .IsRequired(false);
            });

            modelBuilder.Entity<LearningLevel>(entity =>
            {
                entity.HasKey(e => e.LevelId).HasName("PK__Learning__09F03C26EC2A3FB4");
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasKey(e => e.LessonId).HasName("PK__Lessons__B084ACD0F7EAA506");
                entity.Property(e => e.OrderIndex).HasDefaultValue(0);
                entity.HasOne(d => d.Course)
                    .WithMany(p => p.Lessons)
                    .HasConstraintName("FK__Lessons__CourseI__45F365D3")
                    .OnDelete(DeleteBehavior.Cascade); // Xóa Course thì Lesson cũng bị xóa
            });

            // ✅ Thêm cấu hình cho Quiz
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasKey(e => e.QuizId);
                entity.HasOne(q => q.Lesson)
                    .WithMany(l => l.Quizzes)
                    .HasForeignKey(q => q.LessonId)
                    .HasConstraintName("FK_Quizzes_Lessons_LessonId")
                    .OnDelete(DeleteBehavior.Cascade); // Xóa Lesson thì Quiz cũng bị xóa
            });

            modelBuilder.Entity<Progress>(entity =>
            {
                entity.HasKey(e => e.ProgressId).HasName("PK__Progress__BAE29CA5B14894C3");
                entity.Property(e => e.IsCompleted).HasDefaultValue(false);
                entity.HasOne(d => d.Lesson)
                    .WithMany(p => p.Progresses)
                    .HasConstraintName("FK__Progresse__Lesso__4AB81AF0");
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Progresses)
                    .HasConstraintName("FK__Progresse__UserI__49C3F6B7");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CC30BF25F");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Role).HasDefaultValue("User");
            });

           
            modelBuilder.Entity<Paragraph>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Paragraphs)
                    .HasForeignKey(p => p.UserId)
                    .HasConstraintName("FK_Paragraphs_UserId")
                    .OnDelete(DeleteBehavior.Restrict); // Restrict để tránh multiple cascade paths
            });

            modelBuilder.Entity<TranslationHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(h => h.User)
                    .WithMany(u => u.TranslationHistories)
                    .HasForeignKey(h => h.UserId)
                    .HasConstraintName("FK_TranslationHistories_UserId")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(h => h.Paragraph)
                    .WithMany()
                    .HasForeignKey(h => h.ParagraphId)
                    .HasConstraintName("FK_TranslationHistories_ParagraphId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
