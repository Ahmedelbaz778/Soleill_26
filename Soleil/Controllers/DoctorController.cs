using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        var result = doctors.Select(d => new DoctorDto
        {
            Id = d.Id,
            FullName = $"{d.User.FirstName} {d.User.LastName}",
            Education = d.Education,
            ExperienceYears = d.ExperienceYears,
            City = d.City,
            ClinicPhone = d.ClinicPhone,
            WorkingHours = d.WorkingHours,
            ProfileImage = d.CertificateImage
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
            ProfileImage = doctor.CertificateImage
        };

        return Ok(result);
    }
}