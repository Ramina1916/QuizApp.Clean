using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Queries
{
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

}
