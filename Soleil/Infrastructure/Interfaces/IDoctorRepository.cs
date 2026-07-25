using Soleil.Models.DTOs.Doctor;
using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Interfaces;

public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
    
    Task<bool> UpdateDoctorAsync(string userId, UpdateDoctorDto dto);
    Task<Doctor?> GetDoctorByIdAsync(int id);
}