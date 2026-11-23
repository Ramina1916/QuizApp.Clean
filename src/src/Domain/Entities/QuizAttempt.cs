using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Domain.Entities
{
    public class QuizAttempt
    {
        /*
         Represents a user's attempt at taking a Quiz.
         */

        // Private setter properties :
        public int Id { get; private set; }
        public int QuizId { get; private set; }
        public string UserId { get; private set; } = string.Empty;
        public DateTime StartedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public int Score { get; private set; }
        public int MaxScore { get; private set; }
        public Quiz Quiz { get; private set; } = null!;
        public ICollection<AttemptAnswer> Answers { get; private set; } = new List<AttemptAnswer>();

        //  -------------------------------------------------------------------------------------------------------

        // Constructors: 
        private QuizAttempt()
        {
        }

        public QuizAttempt(int quizId,string userId,DateTime startedAt,DateTime expiresAt,int maxScore)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userId);

            if (quizId <= 0)
                throw new ArgumentOutOfRangeException(nameof(quizId));

            if (expiresAt <= startedAt)
                throw new ArgumentException("The expiration time must be after the start time.",
                    nameof(expiresAt));

            if (maxScore < 0)
                throw new ArgumentOutOfRangeException(nameof(maxScore));

            QuizId = quizId;
            UserId = userId.Trim();
            StartedAt = startedAt;
            ExpiresAt = expiresAt;
            MaxScore = maxScore;
        }

        //  -------------------------------------------------------------------------------------------------------

        // Domain methods
        public bool IsCompleted => CompletedAt.HasValue;

        public void Expire()
        {
            if (IsCompleted)
                return;
            CompletedAt = ExpiresAt;
        }

        public void Complete(int score, IEnumerable<AttemptAnswer> answers)
        {
            ArgumentNullException.ThrowIfNull(answers);

            if (IsCompleted)
                throw new InvalidOperationException("The attempt has already been completed.");

            if (DateTime.UtcNow > ExpiresAt)
                throw new InvalidOperationException("The attempt has expired and cannot be completed.");

            if (score < 0 || score > MaxScore)
                throw new ArgumentOutOfRangeException(nameof(score));

            var answerList = answers.ToList();


            if (MaxScore > 0 && answerList.Count == 0)
                throw new InvalidOperationException("Cannot complete a scored attempt with zero answers.");

            Answers.Clear();
            foreach (var answer in answerList)
            {
                ArgumentNullException.ThrowIfNull(answer);
                Answers.Add(answer);
            }

            Score = score;
            CompletedAt = DateTime.UtcNow;
        }


    }
}


