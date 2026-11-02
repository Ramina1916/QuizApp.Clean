using Microsoft.EntityFrameworkCore;
using QuizApp.Application.DTOs.Commands;
using QuizApp.Application.DTOs.Queries;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Enums;
namespace QuizApp.Application.Services
{
    // install the Microsoft.EntityFrameworkCore NuGet package to use the ToListAsync extension method
    public class QuizService : IQuizService
    {
        // Business logic for the QuizService goes here. The QuizService is responsible for handling all the business logic related to quizzes
        private readonly IApplicationDbContext _context;

        // constructor to inject the IApplicationDbContext dependency into the QuizService class.
        // This allows the service to interact with the database context for performing CRUD operations on quizzes and related entities.
        public QuizService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateQuizAsync(CreateQuizCommand command, string userId)
        {
            var quiz = new Domain.Entities.Quiz
            {
                Title = command.Title,
                Description = command.Description,
                Category = command.Category,
                Difficulty = command.Difficulty,
                TimeLimitMinutes = command.TimeLimitMinutes,
                Status = QuizStatus.Draft,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz.Id;
        }

        public async Task<List<QuizSummaryDto>> GetActiveQuizzesAsync()
        {
            return await _context.Quizzes
                .Where(q => q.Status == QuizStatus.Active && !q.IsDeleted)
                .Select(q => new QuizSummaryDto(
                    q.Id,
                    q.Title,
                    q.Description,
                    q.Category,
                    q.Difficulty,
                    q.TimeLimitMinutes,
                    q.Questions.Count
                ))
                .ToListAsync();
        }

        public async Task<QuizDetailsDto?> GetQuizDetailsAsync(int quizId, string userId)
        {
            return  await _context.Quizzes
                 .Where(q => q.Id == quizId && (q.Status == QuizStatus.Active || q.CreatedByUserId == userId))
                 .Select(q => new QuizDetailsDto(
                     q.Id,
                     q.Title,
                     q.Description,
                     q.Category,
                     q.Difficulty,
                     q.TimeLimitMinutes,
                     IsOwner: q.CreatedByUserId == userId, // Computed from the parameter, perfectly clean

                     // FIX: Added the nested Options projection and .ToList() to both levels
                     Questions: q.Questions
                         .Where(qq => !qq.IsDeleted) // Hide deleted questions
                         .OrderBy(qq => qq.DisplayOrder)
                         .Select(qq => new QuestionDto(
                             qq.Id,
                             qq.Text,
                             qq.QuestionType,
                             qq.Points,
                             Options: qq.Options
                                 .Select(o => new AnswerOptionDto(o.Id, o.Text))
                                 .ToList()
                         ))
                         .ToList()
                 ))
                 .FirstOrDefaultAsync(); // used instead of find() to ensure that the query is executed on the database side and returns a single result or null if not found.
            // find() make the query execute on the client side, which can lead to performance issues and unexpected behavior when working with related entities.
        }
    }
}
