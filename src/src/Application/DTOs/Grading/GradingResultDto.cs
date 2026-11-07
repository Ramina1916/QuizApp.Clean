namespace QuizApp.Application.DTOs.Grading
{
    // a record that represents the result of grading a quiz submission
    public record GradingResultDto(
        int EarnedScore, 
        int MaxScore, 
        List<GradedAnswerDto> GradedAnswers);
}
