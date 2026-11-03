using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configurations
{
    public class AttemptAnswerConfiguration : IEntityTypeConfiguration<AttemptAnswer>
    {
        public void Configure(EntityTypeBuilder<AttemptAnswer> builder)
        {
            builder.ToTable("AttemptAnswers");

            builder.HasKey(aa => aa.Id);

            builder.Property(aa => aa.QuestionTextSnapshot)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(aa => aa.SelectedOptionTextSnapshot)
                .HasMaxLength(500);

            
            builder.Property(aa => aa.UserAnswer)
                .HasMaxLength(2000);

            // Configures the relationship between AttemptAnswer and QuizAttempt entities, with cascade delete behavior.
            builder.HasOne(aa => aa.Attempt)
                .WithMany(a => a.Answers)
                .HasForeignKey(aa => aa.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the relationship between AttemptAnswer and Question entities, with restrict delete behavior.
            builder.HasOne<Question>()
                .WithMany()
                .HasForeignKey(aa => aa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
