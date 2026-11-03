using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizApp.Domain.Entities;


namespace QuizApp.Infrastructure.Persistence.EntityConfiguration
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz> // inherits from IEntityTypeConfiguration<Quiz> to provide configuration for the Quiz entity.
    {
        public void Configure(EntityTypeBuilder<Quiz> builder) 
        {
            builder.ToTable("Quizzes"); // Configures the table name for the Quiz entity in the database.

            builder.HasKey(q => q.Id); // Configures the primary key for the Quiz entity.

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(200); // Configures it as required the maximum length of the Title property to be 200 characters.

            builder.Property(q => q.Description)
                .HasMaxLength(1000); // Configures the Description property to have a maximum length of 1000 characters.

            builder.Property(q => q.Category)
                 .IsRequired()
                 .HasMaxLength(50); // Configures the Category property to be required and have a maximum length of 50 characters.

            // Enums stored as integers in the database, so we need to configure the conversion for Difficulty and Status properties.
            builder.Property(q => q.Difficulty)
                .HasConversion<int>();
            builder.Property(q => q.Status)
                .HasConversion<int>();

            builder.HasQueryFilter(q => !q.IsDeleted);

            // Configures the one-to-many relationship between Quiz and Question entities, with cascade delete behavior.
            builder.HasMany(q => q.Questions)
                  .WithOne(qq => qq.Quiz)
                  .HasForeignKey(qq => qq.QuizId)
                  .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior means that when a Quiz is deleted, all related QuizAttempt entities will also be deleted automatically.


            // Configures the one-to-many relationship between Quiz and QuizAttempt entities, with cascade delete behavior.
            builder.HasMany(q => q.Attempts)
                   .WithOne(a => a.Quiz)
                   .HasForeignKey(a => a.QuizId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Configures the relationship between Quiz and IdentityUser (CreatedByUserId) with restrict delete behavior.)
            builder.HasOne<IdentityUser>()
                .WithMany() // It is empty because we don't have a navigation property in IdentityUser for the quizzes they created.
                .HasForeignKey(q => q.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
