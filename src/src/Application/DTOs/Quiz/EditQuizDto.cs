using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Quiz
{
    public record EditQuizDto(
        int Id,
        string Title,
        string? Description,
        string Category,
        DifficultyLevel Difficulty,
        int TimeLimitMinutes
        );
}
