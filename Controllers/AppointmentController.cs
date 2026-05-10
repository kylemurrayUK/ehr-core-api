using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EHRCoreAPI
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private AppointmentService _appointmentService;
        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public ActionResult<List<Appointment>> ListAppointments()
        {
                        
            return Ok(_appointmentService.ListAppointments());
        }

        [HttpGet("{Id}")]
        public ActionResult<ReturnAppointmentDTO> GetAppointment(int id)
        {
            var appointment = _appointmentService.GetAppointmentWithDetails(id);

            if (appointment == null)
            {
                return NotFound($"Appointment with id {id} not found");
            }

            var AppointmentResponse = appointment.ToReturnDTO(appointment.Patient!.ToPatientSummary(), appointment.Clinician!.ToClinicianSummary());

            return Ok(AppointmentResponse);
        }

        [HttpGet]
        public ActionResult<List<ReturnAppointmentDTO>> GetAppointmentsBy( 
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
            
            List<Appointment> queryResult = _appointmentService.GetAppointmentBy(filters);
            
            List<ReturnAppointmentDTO> queryResponse = new List<ReturnAppointmentDTO>();
            foreach (Appointment appointment in queryResult)
            {
                queryResponse.Add(appointment.ToReturnDTO(appointment.Patient.ToPatientSummary(),
                 appointment.Clinician.ToClinicianSummary()));
            }
            return Ok(queryResponse);
        }

        [HttpPost]
        public IActionResult CreateAppointment([FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CreateAppointmentStatus createAppointment = await _appointmentService.AddAppointment(createAppointmentDTO);

            if (!createAppointment.WasSuccessful)
            {
                return BadRequest(createAppointment.Message);
            }
            ReturnAppointmentDTO submittedAppointment = createAppointment.NewAppointment!.ToReturnDTO(createAppointment.NewAppointment!.Patient.ToPatientSummary(),
                                                        createAppointment.NewAppointment!.Clinician.ToClinicianSummary());
            return CreatedAtAction(nameof(GetAppointment), new {id = submittedAppointment.Id}, submittedAppointment);
        }

        [HttpPatch]
        public IActionResult ChangeAppointmentStatus([FromBody] ChangeAppointmentStatusDTO changeAppointmentStatusDTO)
        {
            var changeOutcome = _appointmentService.ChangeAppointmentStatus(changeAppointmentStatusDTO);
            if (changeOutcome.wasSuccessful == false)
            {
                return NotFound(changeOutcome.message);
            }

            return Ok(changeOutcome.message);
        }
    }
}