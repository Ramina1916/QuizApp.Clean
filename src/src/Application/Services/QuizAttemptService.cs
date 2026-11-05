using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using QuizApp.Application.DTOs.Commands;
using QuizApp.Application.DTOs.Queries;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

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

        public async Task<TakeQuizDto?> GetTakeQuizAsync(int quizId)
        {
            return await _context.Quizzes
                .Where(q => q.Status == QuizStatus.Active && q.Id == quizId)
                .Select(q => new TakeQuizDto(
                    QuizId: q.Id,
                    Title: q.Title,
                    TimeLimitMinutes: q.TimeLimitMinutes,
                    Questions: q.Questions
                    .OrderBy(qq => qq.DisplayOrder)
                    .Select(qq => new TakeQuestionDto(
                        qq.Id,
                        qq.Text,
                        qq.QuestionType,
                        qq.Points,
                        Options: qq.Options
                        .Select(opt => new AnswerOptionDto(
                            Id: opt.Id, Text: opt.Text
                        )).ToList()
                    )).ToList()
                )).FirstOrDefaultAsync();
        }

        public async Task<int> SubmitAsync(int quizId, string userId, List<SubmittedAnswerDto> answers)
        {
            var truth = await _context.Quizzes
                .Where(q => q.Id == quizId && q.Status == QuizStatus.Active)
                .Include(q => q.Questions) // eager loading 
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync();
            if (truth is null) throw new InvalidOperationException("Quize is no longer available.");

            var result = _grading.Grade(truth, answers);

            var now = DateTime.UtcNow;

            var attempt = new QuizAttempt
            {
                QuizId = truth.Id,
                UserId = userId,
                StartedAt = now,
                CompletedAt = now,
                Score = result.EarnedScore,
                MaxScore = result.MaxScore
            };

            attempt.Answers = result.GradedAnswers.Select(g => new AttemptAnswer
            {
                QuestionTextSnapshot = g.QuestionTextSnapshot,
                SelectedOptionTextSnapshot = g.SelectedOptionTextSnapshot,
                UserAnswer = g.UserAnswer,
                IsCorrect = g.IsCorrect,
                PointsEarned = g.PointsEarned,
                QuestionId = g.QuestionId,
            }).ToList();

            _context.QuizAttempts.Add(attempt);
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
                .Select( a => new AttemptSummaryDto(
                    AttemptId : a.Id,
                    QuizTitle : a.Quiz.Title,
                    Score : a.Score,
                    MaxScore : a.MaxScore,
                    CompletedAt : a.CompletedAt.Value
                )).OrderByDescending(a => a.CompletedAt)
                .ToListAsync();
        }
    }
}
