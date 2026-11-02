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

    public record QuizDetailsDto(
        /*     
             Used for the Details Page.
             It is "heavy". It includes the actual list of Questions because the user needs to see them
        */
        int Id,
        string Title,
        string? Description,
        string Category,
        DifficultyLevel Difficulty,
        int TimeLimitMinutes,
        bool IsOwner, // This tells the UI whether to show the "Edit" and "Delete" buttons.
        List<QuestionDto> Questions
    );

    public record QuestionDto(
        /* 
         Represents a single question on the screen.
        */
        int Id,
        string Text,
        QuestionType QuestionType,
        int Points,
        List<AnswerOptionDto> Options
    );

    public record AnswerOptionDto(
        /*
        Represents a clickable option for a Multiple Choice question.
        */
        int Id,
        string Text
    );

}
