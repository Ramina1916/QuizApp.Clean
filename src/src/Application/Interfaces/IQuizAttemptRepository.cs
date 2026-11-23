using QuizApp.Domain.Entities;

namespace QuizApp.Application.Interfaces;

public interface IQuizAttemptRepository : IRepository<QuizAttempt>
{
    Task<QuizAttempt?> GetForTakingAsync(
        int attemptId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<QuizAttempt?> GetForSubmissionAsync(
        int attemptId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<QuizAttempt?> GetCompletedAsync(
        int attemptId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<QuizAttempt>> GetHistoryAsync(
        string userId,
        CancellationToken cancellationToken = default);
}