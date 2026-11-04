using QuizApp.Domain.Enums;

namespace QuizApp.Application.DTOs.Queries

{
    public record TakeQuestionDto(
    int QuestionId,
    string Text,
    QuestionType QuestionType,
    int Points,
    List<AnswerOptionDto> Options);

}
