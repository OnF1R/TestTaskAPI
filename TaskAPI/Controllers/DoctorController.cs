using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskAPI.Abstractions;
using TaskAPI.Models;
using TaskAPI.Models.Pagination;

namespace TaskAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int id)
        {
            var doctor = await _doctorRepository.GetForUpdateAsync(id);

            if (doctor != null) 
            {
                return StatusCode(StatusCodes.Status200OK, doctor);
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetListAsync(DoctorSort sort, [FromQuery] PaginationParams @params)
        {
            var doctors = await _doctorRepository.GetListAsync(sort);

            var paginationMetadata = new PaginationMetadata(doctors.Count, @params.Page, @params.ItemsPerPage);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var paginatedDoctors = doctors.Skip((@params.Page - 1) * @params.ItemsPerPage).Take(@params.ItemsPerPage);

            if (paginatedDoctors == null || paginatedDoctors.Count() <= 0)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, paginatedDoctors);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(
            [FromQuery] string fio, 
            [FromQuery] int cabinet, 
            [FromQuery] string specialization, 
            [FromQuery] int section
            )
        {
            var doctor = new Doctor
            {
                FIO = fio,
                Cabinet = new Cabinet() { Number = cabinet },
                Specialization = new Specialization() { Title = specialization },
                Section = new Section() { Number = section }
            };

            var dbDoctor = await _doctorRepository.AddAsync(doctor);

            if (dbDoctor != null)
            {
                return StatusCode(StatusCodes.Status200OK, dbDoctor);
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int id, Doctor doctor)
        {
            var dbDoctor = await _doctorRepository.UpdateAsync(id, doctor);

            if (dbDoctor != null)
            {
                return StatusCode(StatusCodes.Status200OK, dbDoctor);
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var doctor = await _doctorRepository.GetAsync(id);

            (bool status, string message) = await _doctorRepository.DeleteAsync(doctor);

            if (status == false)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return StatusCode(StatusCodes.Status200OK, doctor);
        }
    }
}
