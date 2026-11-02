using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Queries
{
    public record QuizSummaryDto(
    /*           
      Used for the Homepage / List View. 
      It is "lightweight". We only need the QuestionCount, not the actual questions,
     so we don't waste database resources loading them.
    */
    int Id,
    string Title,
    string? Description,
    string Category,
    DifficultyLevel Difficulty,
    int TimeLimitMinutes,
    int QuestionCount
    );

}
