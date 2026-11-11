using QuizApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Application.DTOs.Quiz // Command DTOs are used to encapsulate data for creating or updating entities in the application layer.
{
    //  We use validation attribute because 
    public record CreateQuizDto(
    [Required] string Title,
    string? Description,                                  
    [Required] string Category,
    DifficultyLevel Difficulty,                        
    [Range(1, 120)] int TimeLimitMinutes);
}
