using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTO.Scan.EyeScan;
namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class EyeScanController : ControllerBase
{
    private readonly IEyeScanRepository _eyeScanRepo;
    private readonly ApplicationDbContext _context;

    public EyeScanController(IEyeScanRepository eyeScanRepo, ApplicationDbContext context)
    {
        _eyeScanRepo = eyeScanRepo;
        _context = context;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] EyeScanRequestDto dto)
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

        
        try
        {
            var result = await _eyeScanRepo.AnalyzeAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = ex.Message });
        }
    }
}