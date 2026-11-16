using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using QuizApp.Application.DTOs.Attempt;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.DTOs.Grading;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.Services
{
    public class QuizAttemptService : IQuizAttemptService
    {
        private readonly IApplicationDbContext _context;
        private readonly IQuizGradingService _grading;

        public QuizAttemptService(IApplicationDbContext context, IQuizGradingService grading) // Constructor Injection
        {
            _context = context;
            _grading = grading;
        }

        public async Task<TakeQuizDto?> GetTakeQuizAsync(int attemptId, string userId)
        {
            var attempt = await _context.QuizAttempts
            .Include(a => a.Quiz)
            .ThenInclude(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId && a.CompletedAt == null);

            if (attempt is null)
                return null;


            if (DateTime.UtcNow > attempt.ExpiresAt)
            {
                attempt.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return null;
            }

            return new TakeQuizDto(
                AttemptId: attempt.Id,
                QuizId: attempt.QuizId,
                Title: attempt.Quiz.Title,
                TimeLimitMinutes: attempt.Quiz.TimeLimitMinutes,
                Questions: attempt.Quiz.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new TakeQuestionDto(
                        q.Id,
                        q.Text,
                        q.QuestionType,
                        q.Points,
                        Options: q.Options.Select(opt => new AnswerOptionDto(opt.Id, opt.Text)).ToList()
                    )).ToList()
    );
        }

        public async Task<int> StartAttemptAsync(int quizId, string userId)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions) // Need to include to calculate MaxScore
                .FirstOrDefaultAsync(q => q.Id == quizId && q.Status == QuizStatus.Active);

            if (quiz is null) throw new InvalidOperationException("Quiz not available.");

            var startedAt = DateTime.UtcNow;

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                UserId = userId,
                StartedAt = startedAt,
                ExpiresAt = startedAt.AddMinutes(quiz.TimeLimitMinutes), // CALCULATE DEADLINE HERE
                MaxScore = quiz.Questions.Sum(q => q.Points)
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt.Id;
        }

        public async Task<int> SubmitAsync(int attemptId, string userId, List<SubmittedAnswerDto> answers)
        {
            var attempt = await _context.QuizAttempts
            .Include(a => a.Quiz)
            .ThenInclude(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId && a.CompletedAt == null);

            if (attempt is null)
                throw new InvalidOperationException("Attempt not found, already completed, or you are not authorized.");

            if (DateTime.UtcNow > attempt.ExpiresAt)
            {
                attempt.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                throw new InvalidOperationException("Submission rejected: Time limit exceeded.");
            }

            var result = _grading.Grade(attempt.Quiz, answers);

            attempt.CompletedAt = DateTime.UtcNow;
            attempt.Score = result.EarnedScore;

            attempt.Answers = result.GradedAnswers.Select(g => new AttemptAnswer
            {
                QuestionTextSnapshot = g.QuestionTextSnapshot,
                SelectedOptionTextSnapshot = g.SelectedOptionTextSnapshot,
                UserAnswer = g.UserAnswer,
                IsCorrect = g.IsCorrect,
                PointsEarned = g.PointsEarned,
                QuestionId = g.QuestionId,
            }).ToList();

            await _context.SaveChangesAsync();
            return attempt.Id;
        }

        public async Task<QuizResultDto?> GetResultAsync(int attemptId, string userId)
        {
            return await _context.QuizAttempts
                .Where(a => a.UserId == userId && a.Id == attemptId && a.CompletedAt != null)
                .Select(a => new QuizResultDto(
                    AttemptId: a.Id,
                    QuizTitle: a.Quiz.Title,
                    Score: a.Score,
                    MaxScore: a.MaxScore,
                    CompletedAt: a.CompletedAt.Value,
                    Answers: a.Answers
                     .Select(answers => new GradedAnswerDto(
                         QuestionId: answers.QuestionId,
                         QuestionTextSnapshot: answers.QuestionTextSnapshot,
                         SelectedOptionTextSnapshot: answers.SelectedOptionTextSnapshot,
                         UserAnswer: answers.UserAnswer,
                         IsCorrect: answers.IsCorrect,
                         PointsEarned: answers.PointsEarned
                       )).ToList()
                )).FirstOrDefaultAsync();
        }

        public async Task<List<AttemptSummaryDto>> GetHistoryAsync(string userId)
        {
            return await _context.QuizAttempts
                .Where(a => a.UserId == userId && a.CompletedAt != null)
                .Select(a => new AttemptSummaryDto(
                    AttemptId: a.Id,
                    QuizTitle: a.Quiz.Title,
                    Score: a.Score,
                    MaxScore: a.MaxScore,
                    CompletedAt: a.CompletedAt.Value
                )).OrderByDescending(a => a.CompletedAt)
                .ToListAsync();
        }
    }
}
