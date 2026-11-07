using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Quiz
{
    public record QuizDetailsDto(
        // Represents the details of a quiz, including its questions and other relevant information.
        int Id,
        string Title,
        string? Description,
        string Category,
        DifficultyLevel Difficulty,
        int TimeLimitMinutes,
        bool IsOwner,
        List<QuestionDto> Questions
    );
}
