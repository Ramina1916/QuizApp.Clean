using QuizApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuizApp.Application.DTOs.Quiz
{
    public record EditQuizDto(
        int Id,

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200)] string Title,

        [StringLength(1000)] string? Description,

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50)] string Category,

       [EnumDataType(typeof(DifficultyLevel))] DifficultyLevel Difficulty,

        [Required]
        [Range(1, 120)] int TimeLimitMinutes
        );
}
