using Microsoft.AspNetCore.Mvc;
using Soleil.Models.Entities;
using Soleil.Models.DTOs.Account;
using Soleil.Infrastructure.Interfaces;

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

    // 1. تسجيل الدخول
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _accountRepo.ValidateUserAsync(dto);
        if (user == null)
            return Unauthorized(new { Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة" });

        var token = _accountRepo.GenerateJwtToken(user);

        // ✅ رجّع الـ Role عشان الموبايل يعرف هو Parent ولا Doctor
        var role = await _accountRepo.GetUserRoleAsync(user);

        return Ok(new
        {
            Token = token,
            UserName = $"{user.FirstName} {user.LastName}",
            Role = role,
            Message = "تم تسجيل الدخول بنجاح"
        });
    }

    // 2. تسجيل ولي الأمر
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

        // ✅ تعيين الـ Role
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

    // 3. تسجيل الطبيب
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

        // ✅ تعيين الـ Role
        await _accountRepo.AssignRoleAsync(user, "Doctor");

        // ✅ حفظ الصورة الفعلية
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
            IsVerified = false
        };

        await _accountRepo.AddDoctorAsync(doctor);
        await _accountRepo.SaveChangesAsync();

        return Ok(new { Message = "تم تسجيل طلب الطبيب بنجاح" });
    }
}