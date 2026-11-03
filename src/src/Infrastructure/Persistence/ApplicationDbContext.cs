using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;

namespace QuizApp.Infrastructure.Persistence // Persistence is a layer for data access and storage, responsible for managing the application's data and interacting with the database.
{
    // IdentityDbContext<IdentityUser> = EF Core + ASP.NET Identity (Users, Roles, etc.)
    // IApplicationDbContext = the CONTRACT the Application layer defined.
    // This class is the implementation of the contract defined in the Application layer, and it uses EF Core + ASP.NET Identity to manage users, roles, and other identity-related features.
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } // Constructor that takes DbContextOptions and passes them to the base class constructor.

        // DbSet properties for each entity in the domain model, allowing EF Core to manage these entities in the database.
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AnswerOption> AnswerOptions { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<AttemptAnswer> AttemptAnswers { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call the base class's OnModelCreating method to ensure that the Identity model is configured correctly.
           
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); // Apply all configurations from the current assembly, allowing for a clean separation of entity configurations.
        }
    }
}
