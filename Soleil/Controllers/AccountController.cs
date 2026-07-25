using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.DTOs.Account;
using Soleil.Models.Entities;

namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountRepository _accountRepo;

    public AccountController(IAccountRepository accountRepo)
    {
        _accountRepo = accountRepo;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _accountRepo.ValidateUserAsync(dto);
        if (user == null)
            return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة" });

        var token = _accountRepo.GenerateJwtToken(user);
        var role = await _accountRepo.GetUserRoleAsync(user);

        return Ok(new
        {
            Token = token,
            UserName = $"{user.FirstName} {user.LastName}",
            Role = role,
            Message = "تم تسجيل الدخول بنجاح"
        });
    }

    [HttpPost("register-parent")]
    public async Task<IActionResult> RegisterParent(RegisterParentDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _accountRepo.CreateUserAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _accountRepo.AssignRoleAsync(user, "Parent");

        var parent = new Parent
        {
            UserId = user.Id,
            Relation = dto.Relation
        };

        await _accountRepo.AddParentAsync(parent);
        await _accountRepo.SaveChangesAsync();

        return Ok(new { Message = "تم تسجيل ولي الأمر بنجاح" });
    }
    [HttpPut("update-parent")]
    [Authorize]
    public async Task<IActionResult> UpdateParent([FromForm] UpdateParentDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var result = await _accountRepo.UpdateParentAsync(userId, dto);

        if (!result)
            return NotFound(new { Message = "ولي الأمر غير موجود" });

        return Ok(new { Message = "تم تحديث البيانات بنجاح" });
    }

    [HttpPost("register-doctor")]
    public async Task<IActionResult> RegisterDoctor([FromForm] RegisterDoctorDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _accountRepo.CreateUserAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _accountRepo.AssignRoleAsync(user, "Doctor");

        // ✅ Certificate Image
        string fileName = "default-cert.jpg";
        if (dto.CertificateImage != null)
        {
            var folderPath = Path.Combine("wwwroot", "certificates");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            fileName = Guid.NewGuid() + Path.GetExtension(dto.CertificateImage.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.CertificateImage.CopyToAsync(stream);
        }

        string profileImageName = "default-profile.jpg";
        if (dto.ProfileImage != null)
        {
            var profileFolder = Path.Combine("wwwroot", "profiles");
            if (!Directory.Exists(profileFolder))
                Directory.CreateDirectory(profileFolder);

            profileImageName = Guid.NewGuid() + Path.GetExtension(dto.ProfileImage.FileName);
            var profilePath = Path.Combine(profileFolder, profileImageName);

            using var stream = new FileStream(profilePath, FileMode.Create);
            await dto.ProfileImage.CopyToAsync(stream);
        }

        var doctor = new Doctor
        {
            UserId = user.Id,
            Education = dto.Education,
            ExperienceYears = dto.ExperienceYears,
            NationalId = dto.NationalId,
            City = dto.City,
            Street = dto.Street,
            Building = dto.Building,
            ClinicPhone = dto.ClinicPhone,
            WorkingHours = dto.WorkingHours,
            CertificateImage = fileName,
            ProfileImage = profileImageName,
            IsVerified = false
        };

        await _accountRepo.AddDoctorAsync(doctor);
        await _accountRepo.SaveChangesAsync();

        return Ok(new { Message = "تم تسجيل طلب الطبيب بنجاح" });
    }

    // 🆕 الـ Endpoint الجديد الخاص بنسيان كلمة المرور
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _accountRepo.ForgotPasswordAsync(dto.Email);

        if (result == "NotFound")
            return BadRequest(new { Message = "هذا البريد الإلكتروني غير مسجل لدينا!" });

        if (result == "Error")
            return BadRequest(new { Message = "حدث خطأ أثناء إعادة تعيين كلمة المرور." });

        if (result == "EmailFailed")
            return BadRequest(new { Message = "تم تغيير الباسوورد في قاعدة البيانات ولكن فشل إرسال الإيميل." });

        return Ok(new { Message = "تم إرسال كلمة المرور الجديدة إلى بريدك الإلكتروني بنجاح." });
    }
}