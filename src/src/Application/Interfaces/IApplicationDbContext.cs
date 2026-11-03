using Microsoft.EntityFrameworkCore;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        /* 
         The IApplicationDbContext interface defines the contract for the application's database context.
         It provides access to the DbSet properties for each entity in the domain model, 
         allowing for querying and saving data to the database.
        */


        // We define the DbSet properties for each entity in the domain model, allowing for querying and saving data to the database.
        DbSet<Quiz> Quizzes { get; } 
        DbSet<Question> Questions { get; }
        DbSet<AnswerOption> AnswerOptions { get; }
        DbSet<QuizAttempt> QuizAttempts { get; }
        DbSet<AttemptAnswer> AttemptAnswers { get; }

        // make the parameter optional, so that we can call SaveChangesAsync() without passing a cancellation token.
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // This method is used to save changes made in the context to the database asynchronously. It returns the number of state entries written to the database.
    }
}
