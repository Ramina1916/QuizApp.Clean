namespace QuizApp.Application.DTOs.Quiz // Query DTOs are used to transfer data from the application layer to the presentation layer, specifically for query operations.
{
    public record AnswerOptionDto(
        /*
        Represents a clickable option for a Multiple Choice question.
        */
        int Id,
        string Text
    );

}
