using QuizApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;
namespace QuizApp.Application.DTOs.Quiz
{
    // represent a single question with its options and the correct answer
    public record CreateQuestionDto(
        [Required(ErrorMessage = "Question text is required.")]
        [StringLength(200)]
        string Text,

        [Range(1, 3)]
        QuestionType QuestionType,

        [Range(1, 100)]
        int Points,

        [Required]
        List<OptionSlotDto> Options
        );
}

