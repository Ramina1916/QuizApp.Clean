using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Commands
{
    public record SubmitQuizDto
    (
        int QuizId,
        List<SubmittedAnswerDto> Submissions
     );
}
