using QuizApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Application.DTOs.Commands // Command DTOs are used to encapsulate data for creating or updating entities in the application layer.
{
    //  We use validation attribute because 
    public record CreateQuizDto(
    [Required] string Title,
    string? Description,                                  // nullable = optional, no [Required]
    [Required] string Category,
    DifficultyLevel Difficulty,                           // enum, no attribute needed
    [Range(1, 120)] int TimeLimitMinutes);
}
