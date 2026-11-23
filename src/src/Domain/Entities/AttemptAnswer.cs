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

        // Private setter properties :
        public int Id { get; private set; }
        public int AttemptId { get; private set; }
        public int QuestionId { get; private set; }
        public string QuestionTextSnapshot { get; private set; } = string.Empty;
        public string? SelectedOptionTextSnapshot { get; private set; } // For Multi choice questions and TrueFalse
        public string? UserAnswer { get; private set; }
        public bool IsCorrect { get; private set; }
        public int PointsEarned { get; private set; }
        public QuizAttempt Attempt { get; private set; } = null!;
        //  -------------------------------------------------------------------------------------------------------

        // Constructors: 
        private AttemptAnswer()
        {

        }

        public AttemptAnswer(int questionId, string questionTextSnapshot, string? selectedOptionTextSnapshot, string? userAnswer, bool isCorrect, int pointsEarned)
        {
            ArgumentNullException.ThrowIfNull(questionTextSnapshot);

            questionTextSnapshot = questionTextSnapshot.Trim();
            selectedOptionTextSnapshot = selectedOptionTextSnapshot?.Trim();
            userAnswer = userAnswer?.Trim();

            Validate(
                questionId,
                questionTextSnapshot,
                selectedOptionTextSnapshot,
                userAnswer,
                isCorrect,
                pointsEarned);

            QuestionId = questionId;
            QuestionTextSnapshot = questionTextSnapshot;
            SelectedOptionTextSnapshot = selectedOptionTextSnapshot;
            UserAnswer = userAnswer;
            IsCorrect = isCorrect;
            PointsEarned = pointsEarned;
        }

        private static void Validate(int questionId, string questionTextSnapshot, string? selectedOptionTextSnapshot, string? userAnswer, bool isCorrect, int pointsEarned)
        {
            if (questionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(questionId));

            if (string.IsNullOrWhiteSpace(questionTextSnapshot) ||
                questionTextSnapshot.Length > 1000)
            {
                throw new ArgumentException(
                    "Question snapshot is required and must be 1000 characters or less.",
                    nameof(questionTextSnapshot));
            }

            if (selectedOptionTextSnapshot is not null &&
                selectedOptionTextSnapshot.Length > 500)
            {
                throw new ArgumentException(
                    "Selected option snapshot must be 500 characters or less.",
                    nameof(selectedOptionTextSnapshot));
            }

            if (userAnswer is not null && userAnswer.Length > 2000)
            {
                throw new ArgumentException(
                    "User answer must be 2000 characters or less.",
                    nameof(userAnswer));
            }

            if (pointsEarned < 0)
                throw new ArgumentOutOfRangeException(nameof(pointsEarned));

            if (!isCorrect && pointsEarned != 0)
            {
                throw new InvalidOperationException(
                    "An incorrect answer cannot earn points.");
            }
        }
    }
}



