using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;

namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProgressController : ControllerBase
{
    private readonly IProgressRepository _progressRepo;
    private readonly ApplicationDbContext _context;

    public ProgressController(IProgressRepository progressRepo, ApplicationDbContext context)
    {
        _progressRepo = progressRepo;
        _context = context;
    }

    [HttpGet("child/{childId}/latest-report")]
    public async Task<IActionResult> GetLatestReport(int childId)
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

        var report = await _progressRepo.GetLatestReportAsync(childId);

        if (report == null)
            return NotFound(new { Message = "لا يوجد بيانات لهذا الطفل" });

        return Ok(report);
    }
}