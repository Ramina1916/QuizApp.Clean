using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Commands // Command DTOs are used to encapsulate data for creating or updating entities in the application layer.
{
    public record CreateQuizDto(
        string Title,
        string? Description,
        string Category,
        DifficultyLevel Difficulty,
        int TimeLimitMinutes
    );
}
