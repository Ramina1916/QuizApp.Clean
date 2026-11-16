using QuizApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Quiz
{
    public record EditQuestionDto
    (
        int Id,
        int QuizId,
        string Text,
        QuestionType QuestionType,
        int Points,
        List<OptionSlotDto> Options
    );
}
