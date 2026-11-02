using QuizApp.Application.DTOs.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizApp.Application.Services
{
    public interface IQuizService
    {
        public Task<List<QuizSummaryDto>> GetActiveQuizzesAsync();
    }
}
