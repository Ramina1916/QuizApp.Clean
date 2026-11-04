using QuizApp.Domain.Entities;
using QuizApp.Application.DTOs.Commands;

namespace QuizApp.Application.Interfaces
{
    public interface IQuizGradingService
    {
        /*
         implement grading logic for quiz attempts
         the only service that touches zero database, it just takes in a quiz attempt and returns a score
        */
        GradingResultDto Grade(Quiz quiz, List<SubmittedAnswerDto> submissions); // it takes in a quiz and a list of submitted answers, and returns a grading result
    }
}
