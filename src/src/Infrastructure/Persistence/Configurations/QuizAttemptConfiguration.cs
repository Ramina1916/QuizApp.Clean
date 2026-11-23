using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizApp.Domain.Entities;
using QuizApp.Infrastructure.Identity;

namespace QuizApp.Infrastructure.Persistence.Configurations
{
    public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {

            builder.ToTable("QuizAttempts");

            builder.Navigation(a => a.Quiz)
                .IsRequired(false);

            builder.HasKey(a => a.Id);

            // Configures the relationship between QuizAttempt and Quiz entities, with cascade delete behavior.
            builder.HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the relationship between QuizAttempt and ApplicationUser (UserId) with restrict delete behavior.
            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete behavior means that if a user is deleted, their related QuizAttempt entities will not be deleted automatically, and the deletion will be prevented if there are any related QuizAttempt entities.

            // Configures the one-to-many relationship between QuizAttempt and AttemptAnswer entities, with cascade delete behavior.
            builder.HasMany(a => a.Answers)
                .WithOne(aa => aa.Attempt)
                .HasForeignKey(aa => aa.AttemptId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
