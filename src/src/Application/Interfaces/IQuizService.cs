using QuizApp.Application.DTOs.Quiz;
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
        Task AddQuestionAsync(int quizId, string userId, CreateQuestionDto newQuestion);

        Task<int> RemoveQuestionAsync(int questionId, int quizId, string userId);

        Task RemoveQuizAsync(int quizId, string userId);

        Task PublishAsync(int quizId, string userId);
    }
}
