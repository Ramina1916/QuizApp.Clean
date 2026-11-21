using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities
{
    public class Quiz
    {
        /*
        Represents a quiz that users can take.
        Acts as the parent container for Questions and tracks overall metadata.
        */

        // Private setter properties :
        public int Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string? Description { get; private set; }
        public string Category { get; private set; } = string.Empty;
        public DifficultyLevel Difficulty { get; private set; }
        public int TimeLimitMinutes { get; private set; }
        public QuizStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        // Resource-Based Authorization ↓
        public string CreatedByUserId { get; private set; } = string.Empty;
        // Soft Delete ↓
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public ICollection<Question> Questions { get; private set; } = new List<Question>();
        public ICollection<QuizAttempt> Attempts { get; private set; } = new List<QuizAttempt>();

        //  -------------------------------------------------------------------------------------------------------

        // Constructors: 
        // Parameterless Constructor: Required for EF Core to materialize entities from the DB
        private Quiz()
        {
        }

        // Constructor for validating properties
        public Quiz(string title, string? description, string category, DifficultyLevel difficulty, int timeLimitMinutes, string createdByUserId)
        {

            ArgumentNullException.ThrowIfNull(title);
            ArgumentNullException.ThrowIfNull(category);
            ArgumentNullException.ThrowIfNull(createdByUserId);

            title = title.Trim();
            category = category.Trim();
            createdByUserId = createdByUserId.Trim();

            Validate(title, category, timeLimitMinutes);

            Title = title;
            Description = description?.Trim();
            Category = category;
            Difficulty = difficulty;
            TimeLimitMinutes = timeLimitMinutes;
            CreatedByUserId = createdByUserId;
            CreatedAt = DateTime.UtcNow;
            Status = QuizStatus.Draft;
        }


        //  -------------------------------------------------------------------------------------------------------


        // Domain methods

        public void UpdateDetails(string title, string? description, string category, DifficultyLevel difficulty, int timeLimitMinutes)
        {
            if (Status == QuizStatus.Active) throw new InvalidOperationException("Cannot edit a published quiz.");

            Title = title.Trim();
            Description = description?.Trim();
            Category = category.Trim();
            Difficulty = difficulty;
            TimeLimitMinutes = timeLimitMinutes;

            Validate(title, category, timeLimitMinutes);
        }


        public void Publish()
        {
            if (Status != QuizStatus.Draft)
                throw new InvalidOperationException(
                    "Only draft quizzes can be published.");

            Status = QuizStatus.Active;
        }


        public void Delete()
        {
            if (IsDeleted) return; // Idempotency

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }

        
        // DRY principle
        private static void Validate(string title, string category, int timeLimitMinutes)
        {

            if (string.IsNullOrWhiteSpace(title) || title.Length > 200)
                throw new ArgumentException("Title is required and must be 200 characters or less.", nameof(title));

            if (string.IsNullOrWhiteSpace(category) || category.Length > 50)
                throw new ArgumentException("Category is required and must be 50 characters or less.", nameof(category));

            if (timeLimitMinutes < 1 || timeLimitMinutes > 120)
                throw new ArgumentException("Time limit must be between 1 and 120 minutes.", nameof(timeLimitMinutes));
        }

    }
}


