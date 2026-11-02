using QuizApp.Application.DTOs.Queries;
using QuizApp.Application.DTOs.Commands;

namespace QuizApp.Application.Interfaces
{
    public interface IQuizService
    {
        /*
        business logic for the QuizService goes here.The QuizService is responsible for handling all the business logic related to quizzes,
        such as creating quizzes, retrieving quiz details, and managing quiz attempts.
        */

       Task<List<QuizSummaryDto>> GetActiveQuizzesAsync();
        Task<QuizDetailsDto?> GetQuizDetailsAsync(int quizId, string userId);
        Task<int> CreateQuizAsync(CreateQuizCommand command, string userId);
    }
}
