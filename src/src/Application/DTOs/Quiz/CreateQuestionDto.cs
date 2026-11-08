using QuizApp.Domain.Enums;
namespace QuizApp.Application.DTOs.Quiz
{
    // represent a single question with its options and the correct answer
    public record CreateQuestionDto(string Text, QuestionType QuestionType, int Points, List<OptionSlotDto> Options);
}

