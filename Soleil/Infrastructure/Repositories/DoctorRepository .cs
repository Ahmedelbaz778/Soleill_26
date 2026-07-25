using Microsoft.EntityFrameworkCore;
using Soleil.Infrastructure.Interfaces;
using Soleil.Models.Data;
using Soleil.Models.DTOs.Doctor;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Repositories;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
    {
        return await _context.Doctors
            .Where(d => d.IsVerified == true) 
            .Include(d => d.User)
            .ToListAsync();
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id)
    {
        return await _context.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
    public async Task<bool> UpdateDoctorAsync(string userId, UpdateDoctorDto dto)
    {
        var doctor = await _context.Doctors
            .FirstOrDefaultAsync(d => d.UserId == userId);

        if (doctor == null) return false;

        // تحديث البيانات لو موجودة
        if (dto.Education != null) doctor.Education = dto.Education;
        if (dto.ExperienceYears != null) doctor.ExperienceYears = dto.ExperienceYears.Value;
        if (dto.City != null) doctor.City = dto.City;
        if (dto.Street != null) doctor.Street = dto.Street;
        if (dto.Building != null) doctor.Building = dto.Building;
        if (dto.ClinicPhone != null) doctor.ClinicPhone = dto.ClinicPhone;
        if (dto.WorkingHours != null) doctor.WorkingHours = dto.WorkingHours;

        // تحديث الصورة لو موجودة
        if (dto.ProfileImage != null)
        {
            var profileFolder = Path.Combine("wwwroot", "profiles");
            if (!Directory.Exists(profileFolder))
                Directory.CreateDirectory(profileFolder);

            var profileImageName = Guid.NewGuid() +
                Path.GetExtension(dto.ProfileImage.FileName);
            var profilePath = Path.Combine(profileFolder, profileImageName);

            using var stream = new FileStream(profilePath, FileMode.Create);
            await dto.ProfileImage.CopyToAsync(stream);

            doctor.ProfileImage = profileImageName;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}