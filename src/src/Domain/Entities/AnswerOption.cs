namespace QuizApp.Domain.Entities
{
    public class AnswerOption
    {
        /*
        Represents a possible answer for a Question.
        For MultipleChoice/TrueFalse, these are the visible options.
        For ShortAnswer, the "Correct" option holds the accepted text answer.
        */
        public int Id { get; set; }
        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        public Question Question { get; set; } = null!;
    }
}


