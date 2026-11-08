using Microsoft.EntityFrameworkCore;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
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

        public async Task<int> CreateQuizAsync(CreateQuizDto command, string userId)
        {
            var quiz = new Quiz
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
                .Where(q => q.Status == QuizStatus.Active)
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
            return await _context.Quizzes
                 .Where(q => q.Id == quizId && (q.Status == QuizStatus.Active || q.CreatedByUserId == userId))
                 .Select(q => new QuizDetailsDto(
                     q.Id,
                     q.Title,
                     q.Description,
                     q.Category,
                     q.Difficulty,
                     q.TimeLimitMinutes,
                     IsOwner: q.CreatedByUserId == userId,
                     Questions: q.Questions
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

        public async Task<int> AddQuestionsAsync(int quizId, string userId, List<CreateQuestionDto> newQuestions)
        {
            if (!await _context.Quizzes.AnyAsync(q =>
                    q.Id == quizId && q.CreatedByUserId == userId && q.Status == QuizStatus.Draft))
                throw new InvalidOperationException("Quiz not found or not editable.");

            var order = await _context.Questions.CountAsync(q => q.QuizId == quizId);

            foreach (var question in newQuestions)
            {
                var newQuestion = new Question
                {
                    QuizId = quizId,
                    Text = question.Text,
                    QuestionType = question.QuestionType,
                    DisplayOrder = order++,
                    Points = question.Points,
                    Options = question.Options.Select(o => new AnswerOption
                    {
                        Text = o.Text,
                        IsCorrect = o.IsCorrect   
                    }).ToList()
                };
                _context.Questions.Add(newQuestion); 
            }

            await _context.SaveChangesAsync();
            return quizId;
        }

        public async Task<int> RemoveQuestionAsync(int questionId, int quizId, string userId)
        {
            var question = await _context.Questions
                .Where(q => q.Id == questionId && q.QuizId == quizId
                         && q.Quiz.CreatedByUserId == userId
                         && q.Quiz.Status == QuizStatus.Draft)  
                .FirstOrDefaultAsync();                         

            if (question is null)
                throw new InvalidOperationException("Question not found or not editable.");

            question.IsDeleted = true;
            question.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return quizId;                                    
        }

        public async Task RemoveQuizAsync(int quizId, string userId)
        {
            var quiz = await _context.Quizzes
                .Where(q => q.Id == quizId && q.CreatedByUserId == userId)
                .FirstOrDefaultAsync();                                   

            if (quiz is null)
                throw new InvalidOperationException("Quiz not found or not yours.");

            quiz.IsDeleted = true;
            quiz.DeletedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
