using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.Interfaces;
using System.Security.Claims;

namespace QuizApp.Presentation.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly IQuizService _quizService;
        public QuizzesController(IQuizService quizService)
        {
           _quizService = quizService;
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
            var dto = await _quizService.GetQuizDetailsAsync(id, userId);
            if (dto == null)
            {
                return NotFound();
            }
            return View(dto);
        }


    }
}
