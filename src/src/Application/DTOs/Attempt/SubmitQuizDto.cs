using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Attempt
{
    public record SubmitQuizDto
    (
        int AttemptId,
        List<SubmittedAnswerDto> Submissions
     );
}
