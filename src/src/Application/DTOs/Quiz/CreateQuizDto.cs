using QuizApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Application.DTOs.Quiz
{
    public record CreateQuizDto(
    string Title,
    string? Description,                                  
    string Category,
    DifficultyLevel Difficulty,                        
    int TimeLimitMinutes);
}
