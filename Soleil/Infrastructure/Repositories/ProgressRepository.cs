using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.Dto.Progress;


namespace Soleil.Infrastructure.Repositories;

public class ProgressRepository : IProgressRepository
{
    private readonly ApplicationDbContext _context;

    public ProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ChildReportDto?> GetLatestReportAsync(int childId)
    {
        // 1. جيب بيانات الطفل
        var child = await _context.Children
            .FirstOrDefaultAsync(c => c.Id == childId);

        if (child == null) return null;

        // 2. جيب آخر Eye Scan
        var latestEyeScan = await _context.EyeScanTests
            .Where(e => e.ChildId == childId)
            .OrderByDescending(e => e.Date)
            .FirstOrDefaultAsync();

        // 3. جيب آخر نتائج الاستبيان لكل مجال
        var allResults = await _context.QuestionnaireResults
            .Where(r => r.ChildId == childId)
            .Include(r => r.Field)
            .ToListAsync();

        var latestQuestionnaire = allResults
            .GroupBy(r => r.FieldId)
            .Select(g => g.OrderByDescending(r => r.Date).First())
            .Select(r => new QuestionnaireReportDto
            {
                FieldName = r.Field?.Name ?? "غير معروف",
                Score = r.Score,
                Status = CalculateStatus(r.Score)
            })
            .ToList();

        // 4. حدد المجال الأضعف
        var weakestField = latestQuestionnaire
            .OrderBy(q => q.Score)
            .FirstOrDefault();

        // 5. جيب اللعبة المقترحة للمجال الأضعف
        string? suggestedGame = null;
        if (weakestField != null)
        {
            var field = await _context.QuestionnaireFields
                .FirstOrDefaultAsync(f => f.Name == weakestField.FieldName);

            if (field != null)
            {
                var game = await _context.Games
                    .FirstOrDefaultAsync(g => g.FieldId == field.Id);
                suggestedGame = game?.Name;
            }
        }

        // 6. رجّع الـ Report كامل
        return new ChildReportDto
        {
            ChildName = child.Name,
            LastUpdated = DateTime.UtcNow,

            EyeScan = latestEyeScan == null ? null : new EyeScanReportDto
            {
                AsdProbability = latestEyeScan.AsdProbability,
                TdProbability = latestEyeScan.TdProbability,
                Result = latestEyeScan.Result,
                Confidence = latestEyeScan.Confidence,
                Decision = latestEyeScan.Decision,
                Date = latestEyeScan.Date
            },

            Questionnaire = latestQuestionnaire,
            WeakestField = weakestField?.FieldName,
            SuggestedGame = suggestedGame
        };
    }

    private string CalculateStatus(double score)
    {
        if (score < 30) return "لا توجد مؤشرات مقلقة";
        if (score <= 80) return "يحتاج متابعة";
        return "نسبة مرتفعة - يرجى مراجعة طبيب";
    }
}