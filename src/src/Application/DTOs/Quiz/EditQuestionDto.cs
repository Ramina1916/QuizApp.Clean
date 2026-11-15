using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Quiz
{
    public record EditQuestionDto
    (
        int Id,
        int quizId,
        string Text,

    );
}
