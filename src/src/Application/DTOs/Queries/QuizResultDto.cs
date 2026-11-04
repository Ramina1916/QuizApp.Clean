using QuizApp.Application.DTOs.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.DTOs.Queries
{

    public record QuizResultDto(
    // represents the result of a quiz attempt, including the attempt ID, quiz title, score, maximum score, and completion date.
     int AttemptId,
     string QuizTitle,
     int Score,
     int MaxScore,
     DateTime CompletedAt,
     List<GradedAnswerDto> Answers);
}
