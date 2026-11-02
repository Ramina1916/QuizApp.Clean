namespace QuizApp.Application.DTOs.Queries
{
    public record AnswerOptionDto(
        /*
        Represents a clickable option for a Multiple Choice question.
        */
        int Id,
        string Text
    );

}
