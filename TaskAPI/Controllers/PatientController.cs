using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskAPI.Abstractions;
using TaskAPI.Models.Pagination;
using TaskAPI.Models;

namespace TaskAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientController(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(int id)
        {
            var patient = await _patientRepository.GetForUpdateAsync(id);

            if (patient != null)
            {
                return StatusCode(StatusCodes.Status200OK, patient);
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetListAsync(PatientSort sort, [FromQuery] PaginationParams @params)
        {
            var patients = await _patientRepository.GetListAsync(sort);

            var paginationMetadata = new PaginationMetadata(patients.Count, @params.Page, @params.ItemsPerPage);

            Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

            var paginatedPatients = patients.Skip((@params.Page - 1) * @params.ItemsPerPage).Take(@params.ItemsPerPage);

            if (paginatedPatients == null || paginatedPatients.Count() <= 0)
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
            else
            {
                return StatusCode(StatusCodes.Status200OK, paginatedPatients);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(
            [FromQuery] string name,
            [FromQuery] string surname,
            [FromQuery] string patronymic,
            [FromQuery] string address,
            [FromQuery] DateOnly birthDate,
            [FromQuery] string gender,
            [FromQuery] int section
            )
        {
            var patient = new Patient
            {
                Name = name,
                Surname = surname,
                Patronymic = patronymic,
                Address = address,
                BirthDate = birthDate,
                Gender = gender,
                Section = new Section() { Number = section }
            };

            var dbPatient = await _patientRepository.AddAsync(patient);

            if (dbPatient != null)
            {
                return StatusCode(StatusCodes.Status200OK, dbPatient);
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int id, Patient patient)
        {
            var dbPatient = await _patientRepository.UpdateAsync(id, patient);

            if (dbPatient != null)
            {
                return StatusCode(StatusCodes.Status200OK, dbPatient);
            }
            else
            {
                return StatusCode(StatusCodes.Status204NoContent);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var patient = await _patientRepository.GetAsync(id);

            (bool status, string message) = await _patientRepository.DeleteAsync(patient);

            if (status == false)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, message);
            }

            return StatusCode(StatusCodes.Status200OK, patient);
        }
    }
}
