using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Infrastructure.Persistence.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(qq => qq.Id);

            builder.Property(qq => qq.Text)
                .IsRequired()
                .HasMaxLength(1000);

            // Convert the enum to int 
            builder.Property(qq => qq.QuestionType)
                .HasConversion<int>();

            builder.HasQueryFilter(qq => !qq.IsDeleted);


            // Configures the one-to-many relationship between Question and AnswerOption entities, with cascade delete behavior.
            builder.HasMany(qq => qq.Options)
                .WithOne(ao => ao.Question)
                .HasForeignKey(ao => ao.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
