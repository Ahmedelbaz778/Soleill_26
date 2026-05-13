using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Soleil.Models.Entities;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.Dto.Child;

namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // ✅ كل الـ Endpoints محتاجة توكن
public class ChildController : ControllerBase
{
    private readonly IChildRepository _childRepo;
    private readonly ApplicationDbContext _context;

    public ChildController(IChildRepository childRepo, ApplicationDbContext context)
    {
        _childRepo = childRepo;
        _context = context;
    }

    // 1. إضافة طفل جديد
    [HttpPost("add")]
    public async Task<IActionResult> AddChild(AddChildDto dto)
    {
        // ✅ جيب الـ UserId من التوكن مش من المستخدم
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _context.Parents
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parent == null)
            return Unauthorized(new { Message = "غير مصرح" });

        // ✅ التحقق من الحد الأقصى 3 أطفال (من الـ BRD)
        var childrenCount = await _context.Children
            .CountAsync(c => c.ParentId == parent.Id);

        if (childrenCount >= 3)
            return BadRequest(new { Message = "لا يمكن إضافة أكثر من 3 أطفال" });

        var child = new Child
        {
            Name = dto.Name,
            DateOfBirth = dto.BirthDate,
            Gender = dto.Gender,
            ParentId = parent.Id // ✅ من الـ DB مش من المستخدم
        };

        await _childRepo.AddChildAsync(child);
        await _childRepo.SaveChangesAsync();

        return Ok(new { Message = "تمت إضافة الطفل بنجاح", ChildId = child.Id });
    }

    // 2. عرض أطفال ولي الأمر (من التوكن مش من الـ URL)
    [HttpGet("my-children")]
    public async Task<IActionResult> GetMyChildren()
    {
        // ✅ مش بناخد parentId من الـ URL عشان نفس مشكلة الأمان
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _context.Parents
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parent == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var children = await _childRepo.GetChildrenByParentIdAsync(parent.Id);

        if (!children.Any())
            return NotFound(new { Message = "لا يوجد أطفال مسجلين" });

        return Ok(children);
    }

    // 3. عرض تفاصيل طفل محدد بالـ ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetChild(int id)
    {
        // ✅ استبدل الكود القديم بالكود ده كامل
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var parent = await _context.Parents
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (parent == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var child = await _childRepo.GetChildByIdAsync(id);

        if (child == null)
            return NotFound(new { Message = "الطفل غير موجود" });

        if (child.ParentId != parent.Id)
            return Forbid();

        return Ok(new
        {
            child.Id,
            child.Name,
            child.DateOfBirth,
            child.Gender,
            child.ParentId
        });
    }
}