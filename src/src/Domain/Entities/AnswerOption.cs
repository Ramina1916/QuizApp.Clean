namespace QuizApp.Domain.Entities
{
    public class AnswerOption
    {
        /*
        Represents a possible answer for a Question.
        For MultipleChoice/TrueFalse, these are the visible options.
        For ShortAnswer, the "Correct" option holds the accepted text answer.
        */

        // Private setter properties :
        public int Id { get; private set; }
        public int QuestionId { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }
        public Question Question { get; private set; } = null!;

        //  -------------------------------------------------------------------------------------------------------

        // Constructors: 
        private AnswerOption()
        {
        }

        internal AnswerOption(string text, bool isCorrect)
        {
            ArgumentNullException.ThrowIfNull(text);

            text = text.Trim();

            if (string.IsNullOrWhiteSpace(text) || text.Length > 500)
                throw new ArgumentException(
                    "Option text is required and must be 500 characters or less.",
                    nameof(text));

            Text = text;
            IsCorrect = isCorrect;
        }
    }
}


