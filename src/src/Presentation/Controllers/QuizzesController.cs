using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.DTOs.Attempt;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.Interfaces;
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

            return RedirectToAction(nameof(Details), new { id = newQuizId});
        }
    }
}
