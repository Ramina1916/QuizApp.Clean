using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using QuizApp.Domain.Enums;
namespace QuizApp.Application.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IUnitOfWork _unitOfWork;

        public QuizService(IQuizRepository quizRepository, IUnitOfWork unitOfWork)
        {
            _quizRepository = quizRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateQuizAsync(CreateQuizDto newQuiz, string userId)
        {
            var quiz = new Quiz(
                newQuiz.Title,
                newQuiz.Description,
                newQuiz.Category,
                newQuiz.Difficulty,
                newQuiz.TimeLimitMinutes,
                userId
                );
           
            _quizRepository.Add(quiz);
            await _unitOfWork.SaveChangesAsync();
            return quiz.Id;
        }

        public async Task<List<QuizSummaryDto>> GetActiveQuizzesAsync()
        {
            var quizzes = await _quizRepository.GetActiveQuizzesAsync();

            return quizzes
                .Select(q => new QuizSummaryDto(
                    q.Id,
                    q.Title,
                    q.Description,
                    q.Category,
                    q.Difficulty,
                    q.TimeLimitMinutes,
                    q.Questions.Count
                ))
                .ToList();
        }

        public async Task<QuizDetailsDto?> GetQuizDetailsAsync(int quizId, string userId)
        {
            var quiz = await _quizRepository.GetQuizDetailsAsync(quizId, userId);

            if (quiz is null)
                return null;

            return new QuizDetailsDto(
                quiz.Id,
                quiz.Title,
                quiz.Description,
                quiz.Category,
                quiz.Difficulty,
                quiz.Status,
                quiz.TimeLimitMinutes,
                IsOwner: quiz.CreatedByUserId == userId,
                Questions: quiz.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new QuestionDto(
                        q.Id,
                        q.Text,
                        q.QuestionType,
                        q.Points,
                        Options: q.Options
                            .Select(o => new AnswerOptionDto(o.Id, o.Text))
                            .ToList()
                    ))
                    .ToList()
            );
        }

        public async Task AddQuestionAsync(int quizId, string userId, CreateQuestionDto newQuestion)
        {
            ArgumentNullException.ThrowIfNull(newQuestion);

            var quiz = await _quizRepository
            .GetDraftQuizForOwnerAsync(quizId, userId);

            if (quiz is null)
            {
                throw new InvalidOperationException(
                    "The quiz was not found or is not editable.");
            }

            var validOptions = (newQuestion.Options ?? new List<OptionSlotDto>())
                .Where(o => o != null && !string.IsNullOrWhiteSpace(o.Text))
                .ToList();

            var order = await _quizRepository.GetQuestionCountAsync(quizId);

            var questionEntity = new Question
            {
                QuizId = quizId,
                Text = newQuestion.Text.Trim(),
                QuestionType = newQuestion.QuestionType,
                DisplayOrder = order,
                Points = newQuestion.Points,
                Options = validOptions.Select(o => new AnswerOption
                {
                    Text = o.Text.Trim(),
                    IsCorrect = newQuestion.QuestionType == QuestionType.ShortAnswer
                        ? true
                        : o.IsCorrect
                }).ToList()
            };

            _quizRepository.AddQuestion(questionEntity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<int> RemoveQuestionAsync(int questionId, int quizId, string userId)
        {
            var question = await _quizRepository
                 .GetDraftQuestionForOwnerAsync(questionId, quizId, userId);

            if (question is null)
                throw new InvalidOperationException("Question not found or not editable.");

            question.IsDeleted = true;
            question.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return quizId;
        }

        public async Task RemoveQuizAsync(int quizId, string userId)
        {
            var quiz = await _quizRepository
                 .GetQuizForOwnerAsync(quizId, userId);

            if (quiz is null)
                throw new InvalidOperationException("Quiz not found or not yours.");

            quiz.Delete();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task PublishAsync(int quizId, string userId)
        {
            var quiz = await _quizRepository
                .GetDraftQuizForOwnerAsync(quizId, userId);

            if (quiz is null)
                throw new InvalidOperationException(
                    "Quiz not found, not yours, or already published.");

            quiz.Publish();

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<EditQuizDto?> GetQuizForEditAsync(int quizId, string userId)
        {
            var quiz = await _quizRepository.GetQuizForEditAsync(quizId, userId);

            if (quiz is null)
                return null;

            return new EditQuizDto(
                Id: quiz.Id,
                Title: quiz.Title,
                Description: quiz.Description,
                Category: quiz.Category,
                Difficulty: quiz.Difficulty,
                TimeLimitMinutes: quiz.TimeLimitMinutes
            );
        }

        public async Task UpdateQuizAsync(int quizId, string userId, EditQuizDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var quiz = await _quizRepository.GetDraftQuizForOwnerAsync(quizId, userId);

            if (quiz is null)
                throw new InvalidOperationException(
                    "Quiz not found or you are not authorised to edit it.");

            quiz.UpdateDetails(dto.Title, dto.Description, dto.Category, dto.Difficulty, dto.TimeLimitMinutes);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<EditQuestionDto> GetQuestionForEditAsync(int questionId, int quizId, string userId)
        {
            var question = await _quizRepository.GetQuestionForEditAsync(questionId,quizId, userId);

            if (question is null)
                throw new InvalidOperationException(
                    "Question not found or you are not authorised to edit it.");


            return new EditQuestionDto
            (
                Id: question.Id,
                QuizId: question.QuizId,
                Text: question.Text,
                QuestionType: question.QuestionType,
                Points: question.Points,
                Options: question.Options
                .Select(q => new OptionSlotDto(
                    Text: q.Text,
                    IsCorrect: q.IsCorrect
                )).ToList()
            );
        }

        public async Task UpdateQuestionAsync(int questionId, int quizId, string userId, EditQuestionDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var question = await _quizRepository
                .GetDraftQuestionForOwnerAsync(questionId, quizId, userId);

            if (question is null)
                throw new InvalidOperationException(
                    "Question not found or you are not authorised to edit it.");

            var newOptions = dto.Options
                   .Where(o => !string.IsNullOrWhiteSpace(o.Text))
                   .Select(o => new AnswerOption
                   {
                       Text = o.Text.Trim(),
                       IsCorrect = dto.QuestionType == QuestionType.ShortAnswer ? true : o.IsCorrect
                   }).ToList();

            // Replace the collection in EF Core
            question.Options.Clear();
            foreach (var option in newOptions)
            {
                question.Options.Add(option);
            }

            question.Text = dto.Text.Trim();
            question.QuestionType = dto.QuestionType;
            question.Points = dto.Points;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
