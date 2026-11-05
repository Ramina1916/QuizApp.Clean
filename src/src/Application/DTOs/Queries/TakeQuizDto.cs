using QuizApp.Application.DTOs.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Queries
{
    // represents the data required to take a quiz, including the quiz ID, title, time limit, and the list of submitted answers
    public record TakeQuizDto(
         int QuizId,
         string Title,
         int TimeLimitMinutes,
         List<TakeQuestionDto> Questions
        );
}
