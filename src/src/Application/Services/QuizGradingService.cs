using QuizApp.Application.DTOs.Commands;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.Services
{
    public class QuizGradingService : IQuizGradingService
    {
        public GradingResultDto Grade(Quiz quiz, List<SubmittedAnswerDto> submissions)
        {
            var truthAnswers = quiz.Questions.ToDictionary(q => q.Id); // Create a dictionary of questions for quick lookup
            int maxScore = quiz.Questions.Sum(q => q.Points); // sum of all points in the quiz
            int earnedScore = 0;
            var gradedAnswers = new List<GradedAnswerDto>(); // list of graded answers to return

            foreach (var answer in submissions)
            {
                if (!truthAnswers.TryGetValue(answer.QuestionId, out var question)) // try to find the question in the quiz by its ID
                    continue;

                // Determine if the submitted answer is correct based on the question type
                bool isCorrect;
                string? snapshotText;

                if (question.QuestionType == Domain.Enums.QuestionType.ShortAnswer)
                {
                    // For ShortAnswer, check if the submitted text matches any of the correct options
                    var correctAnswers = question.Options
                        .Where(opt => opt.IsCorrect)
                        .Select(opt => opt.Text.Trim().ToLower()); // trim and lowercase for case-insensitive comparison
                    isCorrect = correctAnswers.Contains((answer.UserAnswer ?? "").Trim().ToLower()); // UserAnswer?? is used to handle null values, defaulting to a space if null
                    snapshotText = answer.UserAnswer;
                }
                else
                {
                    // For MultipleChoice/TrueFalse, check if the submitted option ID is correct
                    var selectedOption = question.Options.FirstOrDefault(o => o.Id == answer.SelectedOptionId);
                    isCorrect = selectedOption?.IsCorrect ?? false;
                    snapshotText = selectedOption?.Text;
                }

                int pts = isCorrect ? question.Points : 0; // assign points based on correctness
                earnedScore += pts;
                gradedAnswers.Add(new GradedAnswerDto(question.Id, question.Text, snapshotText,
                    answer.UserAnswer, isCorrect, pts));
            }

            return new GradingResultDto
            (
                earnedScore,
                maxScore,
                gradedAnswers
            );
        }
    }
}
