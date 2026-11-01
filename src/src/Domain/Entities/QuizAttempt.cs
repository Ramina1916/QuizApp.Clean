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
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string UserId { get; set; } = string.Empty;

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int Score { get; set; }
        public int MaxScore { get; set; }

        public Quiz Quiz { get; set; } = null!;
        public ICollection<AttemptAnswer> Answers { get; set; } = new List<AttemptAnswer>();
    }
}


