using Microsoft.AspNetCore.Mvc;
using EHRCoreAPI.Dtos;
using EHRCoreAPI.Dtos.Output;
using EHRCoreAPI.Mappers;
using EHRCoreAPI.Models;
using EHRCoreAPI.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using EHRCoreAPI.Auth;

namespace EHRCoreAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private AppointmentService _appointmentService;
        private IAuthorizationService _authorizationService;
        public AppointmentController(AppointmentService appointmentService, IAuthorizationService authorizationService)
        {
            _appointmentService = appointmentService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Appointment>>> ListAppointments()
        {
            List<Appointment> allAppointments = await _appointmentService.ListAppointmentsAsync();
            List<ReturnAppointmentDTO> allAppointmentsAsDTOs = new List<ReturnAppointmentDTO>();

            foreach (Appointment appointment in allAppointments)
            {
                allAppointmentsAsDTOs.Add(appointment.ToReturnDTO(appointment.Patient.ToPatientSummary(), appointment.Clinician.ToClinicianSummary()));
            }
                       
            return Ok(allAppointmentsAsDTOs);
        }

        [HttpGet("{Id}")]
        public async Task<ActionResult<ReturnAppointmentDTO>> GetAppointment(int Id)
        {
            var appointment = await _appointmentService.GetAppointmentWithDetailsAsync(Id);

            if (appointment == null)
            {
                return NotFound($"Appointment with id {Id} not found");
            }

            var AppointmentResponse = appointment.ToReturnDTO(appointment.Patient!.ToPatientSummary(), appointment.Clinician!.ToClinicianSummary());

            return Ok(AppointmentResponse);
        }

        [HttpGet]
        public async Task<ActionResult<List<ReturnAppointmentDTO>>> GetAppointmentsBy( 
            [FromQuery] int? patientId,
            [FromQuery] int? clinicianId,
            [FromQuery] string? department,
            [FromQuery] string? patientName,
            [FromQuery] string? clinicianName,
            [FromQuery] AppointmentStatus? status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            int parameterCounter = 0;
            object?[] queryParameters = [patientId, clinicianId, department,
                                         patientName, clinicianName, status];

            foreach (object? parameter in queryParameters)
            {
                if (parameter != null)
                {
                    parameterCounter++;
                }
            }
            if (parameterCounter == 0)
            {
                return BadRequest("No query included");
            }

            FilterParameters filters = new FilterParameters(patientId, clinicianId, department,
                                         patientName, clinicianName, status);
            
            List<Appointment> queryResult = await _appointmentService.GetAppointmentByAsync(filters);
            
            List<ReturnAppointmentDTO> queryResponse = new List<ReturnAppointmentDTO>();
            foreach (Appointment appointment in queryResult)
            {
                queryResponse.Add(appointment.ToReturnDTO(appointment.Patient.ToPatientSummary(),
                appointment.Clinician.ToClinicianSummary()));
            }
            return Ok(queryResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CreateAppointmentStatus createAppointment = await _appointmentService.AddAppointmentAsync(createAppointmentDTO);

            if (!createAppointment.WasSuccessful)
            {
                return BadRequest(createAppointment.Message);
            }
            ReturnAppointmentDTO submittedAppointment = createAppointment.NewAppointment!.ToReturnDTO(createAppointment.NewAppointment!.Patient.ToPatientSummary(),
                                                        createAppointment.NewAppointment!.Clinician.ToClinicianSummary());
            return CreatedAtAction(nameof(GetAppointment), new {id = submittedAppointment.Id}, submittedAppointment);
        }

        [Authorize(Policy = PolicyNames.AdminAndClinician)]
        [HttpPatch]
        public async Task<IActionResult> ChangeAppointmentStatus([FromBody] ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            
            var authResult = await _authorizationService.AuthorizeAsync(
            User, changeAppointmentStatusDTO.Status, PolicyNames.ChangeAppointmentStatus);

            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var changeOutcome = await _appointmentService.ChangeAppointmentStatus(changeAppointmentStatusDTO);
            if (!changeOutcome.wasSuccessful)
            {
                return NotFound(changeOutcome.message);
            }

            return Ok(changeOutcome.message);
        }
    }
}