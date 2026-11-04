using QuizApp.Application.DTOs.Queries;
using QuizApp.Application.DTOs.Commands;

namespace QuizApp.Application.Interfaces
{
    public interface IQuizAttemptService
    {
        // the logic for taking, submitting, viewing results, and accessing history of quiz attempts
        Task<TakeQuizDto?> GetTakeQuizAsync(int quizId);                       // build the form
        Task<int> SubmitAsync(int quizId, string userId, List<SubmittedAnswerDto> answers); // grade+save
        Task<QuizResultDto?> GetResultAsync(int attemptId, string userId);     // results page
        Task<List<AttemptSummaryDto>> GetHistoryAsync(string userId);
    }
}
