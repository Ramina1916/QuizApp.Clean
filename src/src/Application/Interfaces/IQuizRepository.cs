using QuizApp.Domain.Entities;

namespace QuizApp.Application.Interfaces;

public interface IQuizRepository : IRepository<Quiz>
{
    Task<IReadOnlyList<Quiz>> GetActiveQuizzesAsync(
        CancellationToken cancellationToken = default);

    Task<Quiz?> GetQuizDetailsAsync(
        int quizId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Quiz?> GetQuizForOwnerAsync(
        int quizId,
        string userId,
        CancellationToken cancellationToken = default);

    void AddQuestion(Question question);

    Task<Quiz?> GetDraftQuizForOwnerAsync(
    int quizId,
    string userId,
    CancellationToken cancellationToken = default);

    Task<Question?> GetDraftQuestionForOwnerAsync(
        int questionId,
        int quizId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<int> GetQuestionCountAsync(
    int quizId,
    CancellationToken cancellationToken = default);

    Task<Question?> GetQuestionForEditAsync(
        int questionId,
        int quizId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Quiz?> GetQuizForEditAsync(
        int quizId,
        string userId,
        CancellationToken cancellationToken = default);

    Task<Quiz?> GetActiveQuizForAttemptAsync(
    int quizId,
    CancellationToken cancellationToken = default);
}