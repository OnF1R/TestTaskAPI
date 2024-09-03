using TaskAPI.Models;

namespace TaskAPI.Abstractions
{
    public interface IDoctorRepository
    {
        Task<Doctor> GetAsync(int id);
        Task<DoctorForUpdate> GetForUpdateAsync(int id);
        Task<List<DoctorWithoutForeignKeys>> GetListAsync(DoctorSort sort);
        Task<Doctor> AddAsync(Doctor doctor);
        Task<Doctor> UpdateAsync(int id, Doctor doctor);
        Task<(bool, string)> DeleteAsync(Doctor doctor);
    }
}
