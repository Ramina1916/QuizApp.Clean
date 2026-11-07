using QuizApp.Application.DTOs.Quiz;
using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Attempt

{
    public record TakeQuestionDto(
    int QuestionId,
    string Text,
    QuestionType QuestionType,
    int Points,
    List<AnswerOptionDto> Options);

}
