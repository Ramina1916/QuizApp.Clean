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
        Task<int> AddQuestionsAsync(int quizId, string userId, List<CreateQuestionDto> newQuestions);

        Task<int> RemoveQuestionAsync(int questionId, int quizId, string userId);

        Task RemoveQuizAsync(int quizId, string userId);
    }
}
