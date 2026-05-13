using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Assessment;
using Soleil.Models.Entities;

namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AssessmentController : ControllerBase
{
    private readonly IAssessmentRepository _assessmentRepo;
    private readonly ApplicationDbContext _context;

    public AssessmentController(IAssessmentRepository assessmentRepo, ApplicationDbContext context)
    {
        _assessmentRepo = assessmentRepo;
        _context = context;
    }

    // 1. عرض جميع المجالات
    [HttpGet("get-all-fields")]
    public async Task<IActionResult> GetFields()
    {
        var fields = await _assessmentRepo.GetAllFieldsAsync();
        return Ok(fields);
    }

    // 2. عرض الأسئلة بمجال معين
    [HttpGet("get-questions-by-field/{fieldId}")]
    public async Task<IActionResult> GetQuestionsByField(int fieldId)
    {
        var questions = await _assessmentRepo.GetQuestionsByFieldAsync(fieldId);
        if (!questions.Any())
            return NotFound(new { Message = $"لا يوجد أسئلة لهذا المجال (ID: {fieldId})" });
        return Ok(questions);
    }

    // 3. استقبال الإجابات وحفظ النتائج
    [HttpPost("submit-questionnaire")]
    public async Task<IActionResult> Submit(SubmitQuestionnaireDto dto)
    {
        if (dto == null || !dto.Answers.Any())
            return BadRequest("يجب إرسال إجابات صالحة.");

        var allQuestions = await _context.Questions.ToListAsync();

        var resultsByField = dto.Answers
            .Join(allQuestions,
                  ans => ans.QuestionId,
                  quest => quest.Id,
                  (ans, quest) => new { quest.FieldId, ans.Score })
            .GroupBy(x => x.FieldId)
            .Select(g =>
            {
                var totalQuestions = g.Count();
                var maxPossibleScore = totalQuestions * 3; // ✅ الحد الأقصى
                var rawScore = g.Sum(x => x.Score);
                var percentage = (rawScore / (double)maxPossibleScore) * 100; // ✅ نسبة مئوية

                return new QuestionnaireResult
                {
                    ChildId = dto.ChildId,
                    FieldId = g.Key,
                    Score = Math.Round(percentage, 2), // ✅ بيتحفظ كـ %
                    Date = DateTime.UtcNow
                };
            }).ToList();

        if (!resultsByField.Any())
            return BadRequest("فشل في تحليل البيانات.");

        await _assessmentRepo.SubmitQuestionnaireAsync(resultsByField);
        await _assessmentRepo.SaveChangesAsync();

        return Ok(new
        {
            Message = "تم حفظ نتائج التقييم بنجاح",
            Summary = resultsByField.Select(r => new { r.FieldId, Score = $"{r.Score}%" })
        });
    }

    // 4. عرض كل الأسئلة
    [HttpGet("get-all-questions")]
    public async Task<IActionResult> GetAllQuestions()
    {
        var questions = await _assessmentRepo.GetAllQuestionsAsync();
        return Ok(questions);
    }
}