using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence.Configurations
{
    public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {

            builder.ToTable("QuizAttempts");

            builder.HasKey(a => a.Id);


            // Configures the relationship between QuizAttempt and Quiz entities, with cascade delete behavior.
            builder.HasOne(a => a.Quiz)
                .WithMany(q => q.Attempts)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configures the relationship between QuizAttempt and IdentityUser (UserId) with restrict delete behavior.
            builder.HasOne<IdentityUser>()
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
