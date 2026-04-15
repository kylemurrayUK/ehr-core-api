using Microsoft.AspNetCore.Mvc;

namespace AppointmentManagementAPI
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
        public ActionResult<Appointment> GetAppointment(int id)
        {
            var appointment = _appointmentService.GetAppointment(id);

            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }

        //In a typical NHS system an id would be used here - this will be implemented in a later project.
        [HttpGet]
        public ActionResult<List<Appointment>> GetAppointments( 
            [FromQuery] string? patient,
            [FromQuery] string? clinician,
            [FromQuery] string? department)
        {
            int parameterCounter = 0;
            string?[] queryParameters = [patient, clinician, department];

            foreach (string? parameter in queryParameters)
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
            else if (parameterCounter > 1)
            {
                return BadRequest("More than one query parameter not allowed");
            }
            
            List<Appointment> queryResult = new List<Appointment>();

            if (patient != null)
            {
                queryResult = _appointmentService.GetPatientAppointments(patient);
            }
            else if (clinician != null)
            {
                queryResult = _appointmentService.GetClinicianAppointments(clinician);
            }
            else if (department != null)
            {
                queryResult = _appointmentService.GetDepartmentAppointments(department);
            }

            return Ok(queryResult);
        }

        [HttpPost]
        public IActionResult CreateAppointment([FromBody] CreateAppointmentDTO createAppointmentDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Appointment newAppointment =_appointmentService.AddAppointment(createAppointmentDTO);

            return CreatedAtAction(nameof(GetAppointment), new {id = newAppointment.Id}, newAppointment);
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