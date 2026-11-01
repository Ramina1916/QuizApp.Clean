using QuizApp.Domain.Enums;

namespace QuizApp.Domain.Entities
{
    public class Question
    {
        /*
         Represents an individual question within a specific Quiz.
         Includes Soft Delete (IsDeleted) so that deleting a question 
         does not destroy historical QuizAttempt records.
        */
        public int Id { get; set; }
        public int QuizId { get; set; }

        public string Text { get; set; } = string.Empty;
        public QuestionType QuestionType { get; set; }
        public int DisplayOrder { get; set; }
        public int Points { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        public Quiz Quiz { get; set; } = null!;
        public ICollection<AnswerOption> Options { get; set; } = new List<AnswerOption>();
    }
}


