using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Application.DTOs.Attempt;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Entities;
using System.Security.Claims;

namespace QuizApp.Presentation.Controllers
{
    public class QuizzesController : Controller
    {

        private readonly IQuizService _quizService;
        private readonly IQuizAttemptService _quizAttemptService;
        public QuizzesController(IQuizService quizService, IQuizAttemptService quizAttemptService)
        {
            _quizService = quizService;
            _quizAttemptService = quizAttemptService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dtos = await _quizService.GetActiveQuizzesAsync();
            return View(dtos);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            var dto = await _quizService.GetQuizDetailsAsync(id, userId);
            if (dto == null)
            {
                return NotFound();
            }
            return View(dto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Take(int id)
        {
            //
            var dto = await _quizAttemptService.GetTakeQuizAsync(id);
            if (dto == null)
            {
                return NotFound();
            }
            return View(dto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(SubmitQuizDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            var attemptId = await _quizAttemptService.SubmitAsync(model.QuizId, userId, model.Submissions);
            return RedirectToAction("Results", new { id = attemptId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Results(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            var dto = await _quizAttemptService.GetResultAsync(id, userId);
            if (dto is null) return NotFound();
            return View(dto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            var dtos = await _quizAttemptService.GetHistoryAsync(userId);
            return View(dtos);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuizDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();

            var newQuizId = await _quizService.CreateQuizAsync(model, userId);

            return RedirectToAction(nameof(Details), new { id = newQuizId });
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddQuestion(int quizId)
        {
            var model = new CreateQuestionDto(
                Text: "",
                QuestionType: Domain.Enums.QuestionType.MultipleChoice,
                Points: 1,
                Options: new List<OptionSlotDto>
                {
            new OptionSlotDto("", false),
            new OptionSlotDto("", false)
                }
            );

            ViewBag.QuizId = quizId;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(int quizId, CreateQuestionDto question)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Forbid();

            ViewBag.QuizId = quizId;

            if (!ModelState.IsValid)
                return View(question);

            try
            {
                await _quizService.AddQuestionAsync(quizId, userId, question);
                return RedirectToAction(nameof(Details), new { id = quizId });
            }
            catch (InvalidOperationException ex)
            {
                // Display a safe validation message.
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(question);
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQuestion(int questionId, int quizId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            try
            {
                await _quizService.RemoveQuestionAsync(questionId, quizId, userId);
                return RedirectToAction(nameof(Details), new { id = quizId });
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveQuiz(int quizId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            try
            {
                await _quizService.RemoveQuizAsync(quizId, userId);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Forbid();
            try
            {
                await _quizService.PublishAsync(id, userId);
                return RedirectToAction(nameof(Details), new { id }); // Stay on the page to see the change
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Forbid();

            var dto = await _quizService.GetQuizForEditAsync(id, userId);

            if (dto is null)
                return NotFound();

            return View(dto);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditQuizDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Forbid();

            if (!ModelState.IsValid)
                return View(dto with { Id = id });

            try
            {
                await _quizService.UpdateQuizAsync(id, userId, dto);

                TempData["SuccessMessage"] = "Quiz updated successfully.";

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}