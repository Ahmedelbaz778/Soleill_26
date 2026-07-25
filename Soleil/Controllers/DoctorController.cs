using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.DTOs.Doctor;

namespace Soleil.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DoctorController : ControllerBase
{
    private readonly IDoctorRepository _doctorRepo;

    public DoctorController(IDoctorRepository doctorRepo)
    {
        _doctorRepo = doctorRepo;
    }

    // 1. قائمة الدكاترة المعتمدين
    [HttpGet("list")]
    public async Task<IActionResult> GetAllDoctors()
    {
        var doctors = await _doctorRepo.GetAllDoctorsAsync();

        if (!doctors.Any())
            return NotFound(new { Message = "لا يوجد دكاترة متاحون حالياً" });

        var baseUrl = "https://soleilahmeda.runasp.net";

        var result = doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            FullName = $"{d.User.FirstName} {d.User.LastName}",
            Education = d.Education,
            ExperienceYears = d.ExperienceYears,
            City = d.City,
            ClinicPhone = d.ClinicPhone,
            WorkingHours = d.WorkingHours,
            ProfileImage = d.ProfileImage != null
                ? $"{baseUrl}/profiles/{d.ProfileImage}"
                : null,
            //CertificateImage = d.CertificateImage != null
               // ? $"{baseUrl}/certificates/{d.CertificateImage}"
              //  : null
        });

        return Ok(result);
    }

    // 2. بروفايل دكتور معين
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDoctor(int id)
    {
        var doctor = await _doctorRepo.GetDoctorByIdAsync(id);

        if (doctor == null)
            return NotFound(new { Message = "الطبيب غير موجود" });

        var result = new DoctorDto
        {
            Id = doctor.Id,
            FullName = $"{doctor.User.FirstName} {doctor.User.LastName}",
            Education = doctor.Education,
            ExperienceYears = doctor.ExperienceYears,
            City = doctor.City,
            ClinicPhone = doctor.ClinicPhone,
            WorkingHours = doctor.WorkingHours,
            ProfileImage = doctor.ProfileImage,
            CertificateImage = doctor.CertificateImage
        };

        return Ok(result);
    }
    [HttpPut("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateDoctorDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized(new { Message = "غير مصرح" });

        var result = await _doctorRepo.UpdateDoctorAsync(userId, dto);

        if (!result)
            return NotFound(new { Message = "الطبيب غير موجود" });

        return Ok(new { Message = "تم تحديث البيانات بنجاح" });
    }
}