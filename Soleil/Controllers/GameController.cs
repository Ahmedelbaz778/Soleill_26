using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Game;

namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GameController : ControllerBase
{
    private readonly IGameRepository _gameRepo;
    private readonly ApplicationDbContext _context;

    public GameController(IGameRepository gameRepo, ApplicationDbContext context)
    {
        _gameRepo = gameRepo;
        _context = context;
    }

    // 1. جيب اللعبة المقترحة
    [HttpGet("suggested/{childId}")]
    public async Task<IActionResult> GetSuggestedGame(int childId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _context.Parents
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parent == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var child = await _context.Children
            .FirstOrDefaultAsync(c => c.Id == childId && c.ParentId == parent.Id);

        if (child == null)
            return Forbid();

        // تحقق لو محتاج يعيد الاختبار
        var reTestRequired = await _gameRepo.CheckReTestRequiredAsync(childId);
        if (reTestRequired)
            return BadRequest(new { Message = "يجب إعادة الاختبار قبل اللعب" });

        var game = await _gameRepo.GetSuggestedGameAsync(childId);

        if (game == null)
            return NotFound(new { Message = "لا توجد لعبة مقترحة، يرجى إكمال الاستبيان أولاً" });

        return Ok(game);
    }

    // 2. ابدأ جلسة لعب
    // ابدأ جلسة لعب
    [HttpPost("start-session")]
    public async Task<IActionResult> StartSession([FromBody] StartSessionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _context.Parents
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parent == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var child = await _context.Children
            .FirstOrDefaultAsync(c => c.Id == dto.ChildId && c.ParentId == parent.Id);

        if (child == null)
            return Forbid();

        // تحقق لو محتاج يعيد الاختبار
        var reTestRequired = await _gameRepo.CheckReTestRequiredAsync(dto.ChildId);
        if (reTestRequired)
            return BadRequest(new { Message = "يجب إعادة الاختبار قبل اللعب" });

        var session = await _gameRepo.StartSessionAsync(dto.ChildId, dto.GameId);

        return Ok(new
        {
            SessionId = session.Id,
            SessionNumber = session.SessionNumber,
            Message = $"تم بدء الجلسة رقم {session.SessionNumber} من 6"
        });
    }

    // أكمل جلسة لعب
    [HttpPost("complete-session")]
    public async Task<IActionResult> CompleteSession([FromBody] CompleteSessionDto dto)
    {
        var result = await _gameRepo.CompleteSessionAsync(dto.SessionId, dto.DurationInSeconds);
        return Ok(result);
    }

    // جيب حالة الجلسات
    [HttpGet("session-status/{childId}")]
    public async Task<IActionResult> GetSessionStatus(int childId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _context.Parents
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parent == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var child = await _context.Children
            .FirstOrDefaultAsync(c => c.Id == childId && c.ParentId == parent.Id);

        if (child == null)
            return Forbid();

        var status = await _gameRepo.GetSessionStatusAsync(childId);
        return Ok(status);
    }

    // 4. تحقق لو محتاج يعيد الاختبار
    [HttpGet("check-retest/{childId}")]
    public async Task<IActionResult> CheckReTest(int childId)
    {
        var reTestRequired = await _gameRepo.CheckReTestRequiredAsync(childId);

        return Ok(new
        {
            ReTestRequired = reTestRequired,
            Message = reTestRequired
                ? "يجب إعادة الاختبار قبل اللعب"
                : "يمكنك الاستمرار في اللعب"
        });
    }
}