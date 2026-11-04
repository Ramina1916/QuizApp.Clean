namespace QuizApp.Application.DTOs.Commands
{
    // a record that represents a graded answer to a question in a quiz
    public record GradedAnswerDto(
        int QuestionId,
        string QuestionTextSnapshot,   // copied at submit-time → survives future edits
        string? SelectedOptionTextSnapshot,
        string? UserAnswer,
        bool IsCorrect,
        int PointsEarned);
}
