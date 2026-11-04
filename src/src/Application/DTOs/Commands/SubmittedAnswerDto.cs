using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Commands
{
    // a record that represents a submitted answer to a question in a quiz
    // it is considered a command because it is get the data from the user and send it to the application layer to be processed
    public record SubmittedAnswerDto(
        int QuestionId, 
        int? SelectedOptionId,
        string? UserAnswer);
}
