using QuizApp.Application.DTOs.Attempt;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.DTOs.Grading;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;

namespace QuizApp.Application.Services
{
    public class QuizAttemptService : IQuizAttemptService
    {
        private readonly IQuizGradingService _grading;
        private readonly IQuizAttemptRepository _quizAttemptRepository;
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuizAttemptService(IQuizGradingService grading, IQuizAttemptRepository quizAttemptRepository, IUnitOfWork unitOfWork, IQuizRepository quizRepository) // Constructor Injection
        {
            _grading = grading;
            _quizAttemptRepository = quizAttemptRepository;
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<TakeQuizDto?> GetTakeQuizAsync(int attemptId, string userId)
        {
            var attempt = await _quizAttemptRepository.GetForTakingAsync(attemptId, userId);

            if (attempt is null)
                return null;

            if (DateTime.UtcNow > attempt.ExpiresAt)
            {
                attempt.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                return null;
            }

            return new TakeQuizDto(
                AttemptId: attempt.Id,
                QuizId: attempt.QuizId,
                Title: attempt.Quiz.Title,
                ExpiresAt: attempt.ExpiresAt,
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
            var quiz = await _quizRepository.GetActiveQuizForAttemptAsync(quizId);

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

            _quizAttemptRepository.Add(attempt);
            await _unitOfWork.SaveChangesAsync();
            return attempt.Id;
        }

        public async Task<int> SubmitAsync(int attemptId, string userId, List<SubmittedAnswerDto> answers)
        {
            var attempt = await _quizAttemptRepository.GetForSubmissionAsync(attemptId, userId);

            if (attempt is null)
                throw new InvalidOperationException("Attempt not found, already completed, or you are not authorized.");

            if (attempt.CompletedAt is not null)
                return attempt.Id;

            if (DateTime.UtcNow > attempt.ExpiresAt)
            {
                attempt.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();
                throw new InvalidOperationException("Submission rejected: Time limit exceeded.");
            }



            var validQuestionIds = attempt.Quiz.Questions
                .Select(q => q.Id)
                .ToHashSet();

            var submittedQuestionIds = answers.Select(a => a.QuestionId).ToList();

            // heck if the user submitted an answer for attempt Question ID that doesn't belong to this quiz
            if (submittedQuestionIds.Any(id => !validQuestionIds.Contains(id)))
            {
                throw new InvalidOperationException("Submission rejected: Invalid question data detected.");
            }

            // Check for duplicate answers (e.g., attempt manipulated payload sending two answers for Question #1)
            if (submittedQuestionIds.Count != submittedQuestionIds.Distinct().Count())
            {
                throw new InvalidOperationException("Submission rejected: Duplicate answers detected.");
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

            await _unitOfWork.SaveChangesAsync();
            return attempt.Id;
        }

        public async Task<QuizResultDto?> GetResultAsync(int attemptId, string userId)
        {
            var attempt = await _quizAttemptRepository
             .GetCompletedAsync(attemptId, userId);

            if (attempt is null)
                return null;

            return new QuizResultDto(
                    AttemptId: attempt.Id,
                    QuizTitle: attempt.Quiz.Title,
                    Score: attempt.Score,
                    MaxScore: attempt.MaxScore,
                    CompletedAt: attempt.CompletedAt,
                    Answers: attempt.Answers
                     .Select(answers => new GradedAnswerDto(
                         QuestionId: answers.QuestionId,
                         QuestionTextSnapshot: answers.QuestionTextSnapshot,
                         SelectedOptionTextSnapshot: answers.SelectedOptionTextSnapshot,
                         UserAnswer: answers.UserAnswer,
                         IsCorrect: answers.IsCorrect,
                         PointsEarned: answers.PointsEarned
                       )).ToList()
                );
        }

        public async Task<List<AttemptSummaryDto>> GetHistoryAsync(string userId)
        {
            var quizAttempt = await _quizAttemptRepository.GetHistoryAsync(userId);
            return quizAttempt
                .Select(a => new AttemptSummaryDto(
                    AttemptId: a.Id,
                    QuizTitle: a.Quiz.Title,
                    Score: a.Score,
                    MaxScore: a.MaxScore,
                    CompletedAt: a.CompletedAt
                )).ToList();
        }
    }
}
