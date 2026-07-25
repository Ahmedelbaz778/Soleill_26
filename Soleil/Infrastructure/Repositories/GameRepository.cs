using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Game;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Repositories;

public class GameRepository : IGameRepository
{
    private readonly ApplicationDbContext _context;

    public GameRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // 1. جيب اللعبة المقترحة
    public async Task<GameSuggestionDto?> GetSuggestedGameAsync(int childId)
    {
        // جيب أحدث نتيجة لكل مجال
        var latestResults = await _context.QuestionnaireResults
            .AsNoTracking()
            .Where(r => r.ChildId == childId)
            .GroupBy(r => r.FieldId)
            .Select(g => g.OrderByDescending(r => r.Date).FirstOrDefault())
            .Where(r => r != null)
            .Include(r => r!.Field)
            .ToListAsync();

        if (!latestResults.Any()) return null;

        // حدد المجال الأضعف
        var weakestResult = latestResults
            .OrderBy(r => r!.Score)
            .FirstOrDefault();

        if (weakestResult == null) return null;

        // جيب اللعبة المرتبطة بالمجال الأضعف
        var game = await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.FieldId == weakestResult.FieldId);

        if (game == null) return null;

        return new GameSuggestionDto
        {
            GameId = game.Id,
            GameName = game.Name,
            Description = game.Description,
            FieldName = weakestResult.Field.Name,
            FieldScore = weakestResult.Score
        };
    }

    // 2. ابدأ جلسة لعب
    public async Task<GameSession> StartSessionAsync(int childId, int gameId)
    {
        // جيب رقم الجلسة القادمة
        var completedSessions = await _context.GameSessions
            .Where(s => s.ChildId == childId && s.IsCompleted)
            .CountAsync();

        var session = new GameSession
        {
            ChildId = childId,
            GameId = gameId,
            Date = DateTime.UtcNow,
            IsCompleted = false,
            SessionNumber = completedSessions + 1
        };

        await _context.GameSessions.AddAsync(session);
        await _context.SaveChangesAsync();

        return session;
    }

    // 3. أكمل جلسة لعب
    public async Task<SessionStatusDto> CompleteSessionAsync(int sessionId, int durationInSeconds)
    {
        var session = await _context.GameSessions.FindAsync(sessionId);
        if (session == null)
            return new SessionStatusDto { Message = "الجلسة غير موجودة" };

        session.IsCompleted = true;
        session.DurationInSeconds = durationInSeconds;
        await _context.SaveChangesAsync();

        var completedSessions = await _context.GameSessions
            .Where(s => s.ChildId == session.ChildId && s.IsCompleted)
            .CountAsync();

        var reTestRequired = completedSessions >= 6;

        return new SessionStatusDto
        {
            CompletedSessions = completedSessions,
            TotalSessions = 6,
            ReTestRequired = reTestRequired,
            Message = reTestRequired
                ? "🎉 أحسنت! أكملت 6 جلسات، يرجى إعادة الاختبار لقياس التحسن"
                : $"🎮 أحسنت! جلسة {completedSessions} من 6 مكتملة"
        };
    }

    // 4. جيب حالة الجلسات
    public async Task<SessionStatusDto> GetSessionStatusAsync(int childId)
    {
        var completedSessions = await _context.GameSessions
            .AsNoTracking()
            .Where(s => s.ChildId == childId && s.IsCompleted)
            .CountAsync();

        var reTestRequired = completedSessions >= 6;

        return new SessionStatusDto
        {
            CompletedSessions = completedSessions,
            TotalSessions = 6,
            ReTestRequired = reTestRequired,
            Message = reTestRequired
                ? "يجب إعادة الاختبار قبل اللعب"
                : $"جلسة {completedSessions} من 6 مكتملة"
        };
    }

    // 5. التحقق من حاجة الطفل لإعادة الاختبار
    public async Task<bool> CheckReTestRequiredAsync(int childId)
    {
        var completedSessions = await _context.GameSessions
            .AsNoTracking()
            .Where(s => s.ChildId == childId && s.IsCompleted)
            .CountAsync();

        return completedSessions >= 6;
    }
}