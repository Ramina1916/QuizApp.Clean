using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using QuizApp.Application.DTOs.Quiz;

namespace QuizApp.Application.Validators
{
    public class CreateQuizDtoValidator : AbstractValidator<CreateQuizDto>
    {
        public CreateQuizDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .MaximumLength(50).WithMessage("Title cannot exceed 50 characters.");

            RuleFor(x => x.Difficulty)
                .IsInEnum().WithMessage("Invalid difficulty level.");

            RuleFor(x => x.TimeLimitMinutes)
                .InclusiveBetween(1, 120).WithMessage("Time limit must be between 1 and 120 minutes.");
        }
    }
}
