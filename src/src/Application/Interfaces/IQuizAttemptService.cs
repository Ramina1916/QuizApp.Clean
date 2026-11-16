using QuizApp.Application.DTOs.Attempt;

namespace QuizApp.Application.Interfaces
{
    public interface IQuizAttemptService
    {
        // the logic for taking, submitting, viewing results, and accessing history of quiz attempts
        Task<TakeQuizDto?> GetTakeQuizAsync(int attemptId, string userId);                  // build the form
        Task<int> StartAttemptAsync(int quizId, string userId);
        Task<int> SubmitAsync(int attemptId, string userId, List<SubmittedAnswerDto> answers); 
        Task<QuizResultDto?> GetResultAsync(int attemptId, string userId);     // results page
        Task<List<AttemptSummaryDto>> GetHistoryAsync(string userId);
    }
}
