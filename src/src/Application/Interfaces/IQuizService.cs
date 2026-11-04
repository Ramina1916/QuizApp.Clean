using QuizApp.Application.DTOs.Queries;
using QuizApp.Application.DTOs.Commands;

namespace QuizApp.Application.Interfaces
{
    public interface IQuizService
    {
        /*
        business logic for the QuizService goes here:
        List / Details / Create quiz
        */

        Task<List<QuizSummaryDto>> GetActiveQuizzesAsync();
        Task<QuizDetailsDto?> GetQuizDetailsAsync(int quizId, string userId);
        Task<int> CreateQuizAsync(CreateQuizDto command, string userId);
    }
}
