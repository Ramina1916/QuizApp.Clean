using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Commands
{
    public record CreateQuizCommand(
        string Title,
        string? Description,
        string Category,
        DifficultyLevel Difficulty,
        int TimeLimitMinutes
    );
}
