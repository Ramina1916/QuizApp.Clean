namespace QuizApp.Domain.Entities
{
    public class AttemptAnswer
    {
        /* 
        Represents the user's answer to a specific question during a QuizAttempt.
        Uses the Snapshot Pattern: stores the question / option text at the time of the attempt.
        This ensures that if an admin edits or deletes the original Question later, 
        the user's historical attempt record remains accurate and intact.
        */
        public int Id { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }

        // The Snapshot Pattern
        public string QuestionTextSnapshot { get; set; } = string.Empty;
        public string? SelectedOptionTextSnapshot { get; set; } // For Multi choice questions and TrueFalse
        public string? UserAnswer { get; set; }

        public bool IsCorrect { get; set; }
        public int PointsEarned { get; set; }

        public QuizAttempt Attempt { get; set; } = null!;
    }
}


