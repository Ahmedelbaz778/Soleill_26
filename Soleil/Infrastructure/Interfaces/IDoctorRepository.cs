using Soleil.Models.Entities;

namespace Soleil.Infrastructure.Interfaces;

public interface IDoctorRepository
{
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync();
    Task<Doctor?> GetDoctorByIdAsync(int id);
}