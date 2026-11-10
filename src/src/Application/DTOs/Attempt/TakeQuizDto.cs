namespace QuizApp.Application.DTOs.Attempt
{
    // represents the data required to take a quiz, including the quiz ID, title, time limit, and the list of submitted answers
    public record TakeQuizDto(
         int QuizId,
         string Title,
         int TimeLimitMinutes,
         List<TakeQuestionDto> Questions
        );
}
