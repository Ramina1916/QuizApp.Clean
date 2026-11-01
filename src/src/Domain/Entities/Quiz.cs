using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities
{
    public class Quiz
    {
        /*
        Represents a quiz that users can take.
        Acts as the parent container for Questions and tracks overall metadata.
        */
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public int TimeLimitMinutes { get; set; }
        public QuizStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Resource-Based Authorization
        public string CreatedByUserId { get; set; } = string.Empty;

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<QuizAttempt> Attempts { get; set; } = new List<QuizAttempt>();
    }
}


