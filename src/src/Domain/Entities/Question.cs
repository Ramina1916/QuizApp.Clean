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

        // Private setter properties :
        public int Id { get; private set; }
        public int QuizId { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public QuestionType QuestionType { get;  private set; }
        public int DisplayOrder { get; private set; }
        public int Points { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public Quiz Quiz { get; private set; } = null!;
        public ICollection<AnswerOption> Options { get; private set; } = new List<AnswerOption>();

        //  -------------------------------------------------------------------------------------------------------

        // Constructors: 
        private Question()
        {

        }

       public Question(int quizId, string text, QuestionType questionType, int displayOrder ,int points)
       {
            ArgumentNullException.ThrowIfNull(text); 

            if (quizId <= 0)
                throw new ArgumentOutOfRangeException(nameof(quizId));

            if (displayOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(displayOrder));

            text = text.Trim();

            Validate(text, questionType, points);

            QuizId = quizId;
            Text = text;
            QuestionType = questionType;
            Points = points;
            DisplayOrder = displayOrder;
            IsDeleted = false;
       }

        //  -------------------------------------------------------------------------------------------------------


        // Domain methods
        public void UpdateDetails(string text, int points, QuestionType questionType)
        {
            ArgumentNullException.ThrowIfNull(text);

            text = text.Trim();

            Validate(text, questionType, points);

            Text = text;
            QuestionType = questionType;
            Points = points;
        }

        public void AddOption(AnswerOption option)
        {
            ArgumentNullException.ThrowIfNull(option);
            Options.Add(option);
        }
        public void RemoveOption(AnswerOption option)
        {
            ArgumentNullException.ThrowIfNull(option);
            Options.Remove(option);
        }
        public void Delete()
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
        }


        //  -------------------------------------------------------------------------------------------------------

        private static void Validate(string text, QuestionType questionType, int points)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length > 1000)
                throw new ArgumentException(
                    "Question text is required and must be 1000 characters or less.",
                    nameof(text));

            if (!Enum.IsDefined(questionType) ||
                questionType == QuestionType.Unspecified)
            {
                throw new ArgumentException(
                    "A valid question type is required.",
                    nameof(questionType));
            }

            if (points < 1 || points > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(points),
                    "Points must be between 1 and 100.");
        }
    }
}


