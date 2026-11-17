using Microsoft.EntityFrameworkCore;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;

namespace QuizApp.Infrastructure.Persistence.Repositories;

public class QuizRepository : Repository<Quiz>, IQuizRepository
{
    private readonly ApplicationDbContext _context;

    public QuizRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Quiz>> GetActiveQuizzesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .Where(q => q.Status == QuizStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<Quiz?> GetQuizDetailsAsync(
        int quizId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .Where(q =>
                q.Id == quizId &&
                (q.Status == QuizStatus.Active ||
                 q.CreatedByUserId == userId))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Question?> GetDraftQuestionForOwnerAsync(
    int questionId,
    int quizId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .Include(q => q.Options)
            .FirstOrDefaultAsync(
                q => q.Id == questionId &&
                     q.QuizId == quizId &&
                     q.Quiz.CreatedByUserId == userId &&
                     q.Quiz.Status == QuizStatus.Draft,
                cancellationToken);
    }

    public async Task<Quiz?> GetQuizForEditAsync(
    int quizId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                q => q.Id == quizId &&
                     q.CreatedByUserId == userId &&
                     q.Status == QuizStatus.Draft,
                cancellationToken);
    }

    public async Task<Question?> GetQuestionForEditAsync(
    int questionId,
    int quizId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Questions
            .AsNoTracking()
            .Include(q => q.Options)
            .FirstOrDefaultAsync(
                q => q.Id == questionId &&
                     q.QuizId == quizId &&
                     q.Quiz.CreatedByUserId == userId &&
                     q.Quiz.Status == QuizStatus.Draft,
                cancellationToken);
    }

    public Task<int> GetQuestionCountAsync(
    int quizId,
    CancellationToken cancellationToken = default)
    {
        return _context.Questions
            .CountAsync(q => q.QuizId == quizId, cancellationToken);
    }

    public Task<Quiz?> GetQuizForOwnerAsync(
        int quizId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        return _context.Quizzes
            .FirstOrDefaultAsync(
                q => q.Id == quizId &&
                     q.CreatedByUserId == userId,
                cancellationToken);
    }

    public async Task<Quiz?> GetDraftQuizForOwnerAsync(
    int quizId,
    string userId,
    CancellationToken cancellationToken = default)
    {
        return await _context.Quizzes
            .FirstOrDefaultAsync(
                q => q.Id == quizId &&
                     q.CreatedByUserId == userId &&
                     q.Status == QuizStatus.Draft,
                cancellationToken);
    }

    public void AddQuestion(Question question)
    {
        _context.Questions.Add(question);
    }

    public Task<Quiz?> GetActiveQuizForAttemptAsync(int quizId, CancellationToken cancellationToken = default)
    {
        return _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Questions)
            .FirstOrDefaultAsync(
                q => q.Id == quizId &&
                     q.Status == QuizStatus.Active,
                cancellationToken);
    }
}
