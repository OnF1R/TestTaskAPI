using Microsoft.EntityFrameworkCore;
using System.Net;
using TaskAPI.Abstractions;
using TaskAPI.Database;
using TaskAPI.Models;

namespace TaskAPI.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationContext _context;

        public PatientRepository(ApplicationContext context) 
        {
            _context = context;
        }


        public async Task<Patient> GetAsync(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);

                return patient;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<PatientForUpdate> GetForUpdateAsync(int id)
        {
            try
            {
                var findPatient = await _context.Patients
                    .Include(patient => patient.Section)
                    .Where(patient => patient.Id == id)
                    .FirstOrDefaultAsync();

                if (findPatient == null)
                    return null;

                var patientForUpdate = new PatientForUpdate()
                {
                    Name = findPatient.Name,
                    Surname = findPatient.Surname,
                    Patronymic = findPatient.Patronymic,
                    BirthDate = findPatient.BirthDate,
                    Address = findPatient.Address,
                    Gender = findPatient.Gender,
                    SectionId = findPatient.Section.Id,
                };

                return patientForUpdate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<PatientWithoutForeignKeys>> GetListAsync(PatientSort sort)
        {
            try
            {
                var patients = _context.Patients
                    .Include(patient => patient.Section)
                    .AsQueryable()
                    .AsNoTracking();

                patients = sort switch
                {
                    PatientSort.Name => patients.OrderBy(patient => patient.Name),
                    PatientSort.Surname => patients.OrderBy(patient => patient.Surname),
                    PatientSort.Patronymic => patients.OrderBy(patient => patient.Patronymic),
                    PatientSort.Address => patients.OrderBy(patient => patient.Address),
                    PatientSort.Gender => patients.OrderBy(patient => patient.Gender),
                    PatientSort.BirthDate => patients.OrderBy(patient => patient.BirthDate),
                    PatientSort.Section => patients.Where(patient => patient.Section != null).OrderBy(patient => patient.Section.Number),
                    _ => patients.OrderBy(patient => patient.Id),
                };

                var query = from patient in patients
                            join section in _context.Sections on patient.Section.Id equals section.Id
                            select new PatientWithoutForeignKeys
                            {
                                Id = patient.Id,
                                Name = patient.Name,
                                Surname = patient.Surname,
                                Patronymic = patient.Patronymic,
                                BirthDate = patient.BirthDate,
                                Address = patient.Address,
                                Gender = patient.Gender,
                                Section = section.Number,
                            };

                var listPatients = await query.ToListAsync();

                return listPatients;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Patient> AddAsync(Patient patient)
        {
            try
            {
                await _context.AddAsync(patient);
                await _context.SaveChangesAsync();

                return await _context.Patients.FindAsync(patient.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Patient> UpdateAsync(int id, Patient updatedPatient)
        {
            try
            {
                var patient = await _context.Patients.Where(patient => patient.Id == updatedPatient.Id).FirstOrDefaultAsync();

                if (patient == null)
                    return null;

                patient.Name = updatedPatient.Name;
                patient.Surname = updatedPatient.Surname;
                patient.Patronymic = updatedPatient.Patronymic;
                patient.Address = updatedPatient.Address;
                patient.Gender = updatedPatient.Gender;
                patient.BirthDate = updatedPatient.BirthDate;
                patient.Section = updatedPatient.Section;

                _context.Update(patient);

                await _context.SaveChangesAsync();

                return patient;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool, string)> DeleteAsync(Patient patient)
        {
            try
            {
                var dbPatient = await _context.Patients.FindAsync(patient.Id);

                if (dbPatient == null)
                    return (false, "Patient could not be found");

                _context.Remove(dbPatient);
                await _context.SaveChangesAsync();

                return (true, "Patient got deleted");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}
