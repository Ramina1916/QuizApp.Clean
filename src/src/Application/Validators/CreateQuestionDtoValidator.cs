using FluentValidation;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
namespace QuizApp.Application.Validators
{
    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator()
        {

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Question text is required.")
                .MaximumLength(1000);

            RuleFor(x => x.Points)
                .GreaterThan(0).WithMessage("Point must be greater than 0.");

            RuleFor(x => x.QuestionType)
                .IsInEnum().WithMessage("Invalid question type.");

            RuleFor(x => x.Options)
                .NotNull().WithMessage("Options cannot be null.");

            // Multiple Choice Validation
            RuleFor(x => x.Options)
                .Must(options => options != null && options.Count(o => !string.IsNullOrWhiteSpace(o.Text)) >= 2)
                .When(x => x.QuestionType == QuestionType.MultipleChoice)
                .WithMessage("Multiple Choice requires at least two valid options.");

            RuleFor(x => x.Options)
                .Must(options => options != null && options.Count(o => o.IsCorrect) == 1)
                .When(x => x.QuestionType == QuestionType.MultipleChoice)
                .WithMessage("Multiple Choice must have exactly ONE correct answer.");


            // Short Answer Validation
            RuleFor(x => x.Options)
                .Must(options => options != null && options.Any(o => !string.IsNullOrWhiteSpace(o.Text)))
                .When(x => x.QuestionType == QuestionType.ShortAnswer)
                .WithMessage("Short Answer requires at least one acceptable answer.");


            // True/False Validation
            RuleFor(x => x.Options)
                .Must(options => options != null && options.Count(o => o.IsCorrect) == 1)
                .When(x => x.QuestionType == QuestionType.TrueFalse)
                .WithMessage("True/False must have exactly one correct answer.");
        }
    }
}
