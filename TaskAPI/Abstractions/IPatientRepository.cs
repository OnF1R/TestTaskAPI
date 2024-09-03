using TaskAPI.Models;

namespace TaskAPI.Abstractions
{
    public interface IPatientRepository
    {
        Task<Patient> GetAsync(int id);
        Task<PatientForUpdate> GetForUpdateAsync(int id);
        Task<List<PatientWithoutForeignKeys>> GetListAsync(PatientSort sort);
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(int id, Patient patient);
        Task<(bool, string)> DeleteAsync(Patient patient);
    }
}
