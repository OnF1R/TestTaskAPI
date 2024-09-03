using Microsoft.EntityFrameworkCore;
using TaskAPI.Abstractions;
using TaskAPI.Database;
using TaskAPI.Models;

namespace TaskAPI.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationContext _context;

        public DoctorRepository(ApplicationContext context) 
        {
            _context = context;
        }

        public async Task<Doctor> GetAsync(int id)
        {
            try
            {
                var doctor = await _context.Doctors.FindAsync(id);

                return doctor;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<DoctorForUpdate> GetForUpdateAsync(int id)
        {
            try
            {
                var findDoctor = await _context.Doctors
                    .Include(doctor => doctor.Cabinet)
                    .Include(doctor => doctor.Specialization)
                    .Include(doctor => doctor.Section)
                    .Where(doctor => doctor.Id == id)
                    .FirstOrDefaultAsync();

                if (findDoctor == null)
                    return null;

                var doctorForUpdate = new DoctorForUpdate()
                {
                    FIO = findDoctor.FIO,
                    CabinetId = findDoctor.Cabinet.Id,
                    SectionId = findDoctor.Section.Id,
                    SpecializationId = findDoctor.Specialization.Id,
                };

                return doctorForUpdate;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<DoctorWithoutForeignKeys>> GetListAsync(DoctorSort sort)
        {
            try
            {
                var doctors = _context.Doctors
                    .Include(doctor => doctor.Cabinet)
                    .Include(doctor => doctor.Specialization)
                    .Include(doctor => doctor.Section)
                    .AsQueryable()
                    .AsNoTracking();

                doctors = sort switch
                {
                    DoctorSort.FIO => doctors.OrderBy(doctor => doctor.FIO),
                    DoctorSort.Cabinet => doctors.OrderBy(doctor => doctor.Cabinet.Number),
                    DoctorSort.Specialization => doctors.OrderBy(doctor => doctor.Specialization.Title),
                    DoctorSort.Section => doctors.Where(doctor => doctor.Section != null).OrderBy(doctor => doctor.Section.Number),
                    _ => doctors.OrderBy(doctor => doctor.Id),
                };

                var query = from doctor in doctors
                            join section in _context.Sections on doctor.Section.Id equals section.Id
                            join cabinet in _context.Cabinets on doctor.Cabinet.Id equals cabinet.Id
                            join specialization in _context.Specializations on doctor.Specialization.Id equals specialization.Id
                            select new DoctorWithoutForeignKeys
                            {
                                Id = doctor.Id,
                                FIO = doctor.FIO,
                                Specialization = specialization.Title,
                                Cabinet = cabinet.Number,
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

        public async Task<Doctor> AddAsync(Doctor doctor)
        {
            try
            {
                await _context.AddAsync(doctor);
                await _context.SaveChangesAsync();

                return await _context.Doctors.FindAsync(doctor.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Doctor> UpdateAsync(int id, Doctor updatedDoctor)
        {
            try
            {
                var doctor = await _context.Doctors.Where(doctor => doctor.Id == updatedDoctor.Id).FirstOrDefaultAsync();

                if (doctor == null)
                    return null;

                doctor.FIO = updatedDoctor.FIO;
                doctor.Cabinet = updatedDoctor.Cabinet;
                doctor.Specialization = updatedDoctor.Specialization;
                doctor.Section = updatedDoctor.Section;

                _context.Update(doctor);

                await _context.SaveChangesAsync();

                return doctor;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool, string)> DeleteAsync(Doctor doctor)
        {
            try
            {
                var dbDoctor = await _context.Doctors.FindAsync(doctor.Id);

                if (dbDoctor == null)
                    return (false, "Doctor could not be found");

                _context.Remove(dbDoctor);
                await _context.SaveChangesAsync();

                return (true, "Doctor got deleted");
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}");
            }
        }
    }
}
