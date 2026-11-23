using Microsoft.EntityFrameworkCore;
using QuizApp.Application.DTOs.Attempt;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Infrastructure.Persistence.Repositories
{
    public class QuizAttemptRepository : Repository<QuizAttempt>, IQuizAttemptRepository
    {
        private readonly ApplicationDbContext _context;
        public QuizAttemptRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<QuizAttempt?> GetForTakingAsync(int attemptId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizAttempts
             .Include(a => a.Quiz)
             .ThenInclude(q => q.Questions)
             .ThenInclude(q => q.Options)
             .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId, cancellationToken);
        }

        public async Task<QuizAttempt?> GetForSubmissionAsync(int attemptId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizAttempts
            .Include(a => a.Quiz)
            .ThenInclude(q => q.Questions)
            .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId, cancellationToken);
        }

        public async Task<QuizAttempt?> GetCompletedAsync(int attemptId, string userId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizAttempts
             .AsNoTracking()
             .Include(a => a.Quiz)
             .Include(a => a.Answers)
             .FirstOrDefaultAsync(
                 a => a.Id == attemptId &&
                      a.UserId == userId &&
                      a.CompletedAt != null,
                 cancellationToken);
        }

        public async Task<IReadOnlyList<QuizAttempt>> GetHistoryAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.QuizAttempts
            .AsNoTracking()
            .Include(a => a.Quiz)
            .Where(a => a.UserId == userId && a.CompletedAt != null)
            .OrderByDescending(a => a.CompletedAt)
            .ToListAsync(cancellationToken);
        }

    }
}
