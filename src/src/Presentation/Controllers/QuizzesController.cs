using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizApp.Application.DTOs.Attempt;
using QuizApp.Application.DTOs.Quiz;
using QuizApp.Application.Interfaces;
using QuizApp.Domain.Enums;
using QuizApp.Presentation.Extensions;

namespace QuizApp.Presentation.Controllers;

[Authorize]
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
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var dtos = await _quizService.GetActiveQuizzesAsync();
        return View(dtos);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.GetUserId();

        var dto = await _quizService.GetQuizDetailsAsync(id, userId);
        if (dto is null) return NotFound();

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(int quizId)
    {
        var userId = User.GetUserId();

        try
        {
            var attemptId = await _quizAttemptService.StartAttemptAsync(quizId, userId);
            return RedirectToAction(nameof(Take), new { attemptId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Take(int attemptId)
    {
        var userId = User.GetUserId();

        var dto = await _quizAttemptService.GetTakeQuizAsync(attemptId, userId);
        if (dto is null) return RedirectToAction(nameof(TimeExpired));

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(SubmitQuizDto model)
    {
        var userId = User.GetUserId();

        try
        {
            var attemptId = await _quizAttemptService.SubmitAsync(model.AttemptId, userId, model.Submissions);
            return RedirectToAction(nameof(Results), new { id = attemptId });
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("Time limit exceeded"))
            {
                return RedirectToAction(nameof(TimeExpired));
            }

            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult TimeExpired() => View();

    [HttpGet]
    public async Task<IActionResult> Results(int id)
    {
        var userId = User.GetUserId();

        var dto = await _quizAttemptService.GetResultAsync(id, userId);
        if (dto is null) return NotFound();

        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var userId = User.GetUserId();

        var dtos = await _quizAttemptService.GetHistoryAsync(userId);
        return View(dtos);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateQuizDto model)
    {
        if (!ModelState.IsValid) return View(model);

        var userId = User.GetUserId();

        var newQuizId = await _quizService.CreateQuizAsync(model, userId);
        return RedirectToAction(nameof(Details), new { id = newQuizId });
    }

    [HttpGet]
    public IActionResult AddQuestion(int quizId)
    {
        var model = new CreateQuestionDto(
            Text: "",
            QuestionType: QuestionType.MultipleChoice,
            Points: 1,
            Options: new List<OptionSlotDto>
            {
                new OptionSlotDto("", false),
                new OptionSlotDto("", false)
            });

        ViewBag.QuizId = quizId;

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuestion(int quizId, CreateQuestionDto question)
    {
        ViewBag.QuizId = quizId;

        if (!ModelState.IsValid) return View(question);

        var userId = User.GetUserId();

        try
        {
            await _quizService.AddQuestionAsync(quizId, userId, question);
            return RedirectToAction(nameof(Details), new { id = quizId });
        }
        catch (InvalidOperationException ex)
        {
            // Show a safe validation message.
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(question);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveQuestion(int questionId, int quizId)
    {
        var userId = User.GetUserId();

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveQuiz(int quizId)
    {
        var userId = User.GetUserId();

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
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var userId = User.GetUserId();

        try
        {
            await _quizService.PublishAsync(id, userId);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.GetUserId();

        var dto = await _quizService.GetQuizForEditAsync(id, userId);
        if (dto is null) return NotFound();

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditQuizDto dto)
    {
        if (!ModelState.IsValid) return View(dto with { Id = id });

        var userId = User.GetUserId();

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

    [HttpGet]
    public async Task<IActionResult> EditQuestion(int quizId, int id)
    {
        var userId = User.GetUserId();

        var dto = await _quizService.GetQuestionForEditAsync(id, quizId, userId);
        if (dto is null) return NotFound();

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditQuestion(int quizId, int id, EditQuestionDto dto)
    {
        if (!ModelState.IsValid) return View(dto with { QuizId = quizId });

        var userId = User.GetUserId();

        try
        {
            await _quizService.UpdateQuestionAsync(id, quizId, userId, dto);

            TempData["SuccessMessage"] = "Question updated successfully.";
            return RedirectToAction(nameof(Details), new { id = quizId });
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}