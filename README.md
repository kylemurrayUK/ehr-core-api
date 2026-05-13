# ehr-core-api

## Overview
This is an EHR appointment management API that tracks appointments along with linking them to clinicians and patients. The key concept for this project was the introduction of an external SQL database using Entity Framework Core, however other technical highlights were implemented that are detailed below. This is the 4th project on my C# backend development roadmap.  

## Features
Users are able to 
 - Create an appointment
 - View all appointments
 - View a specific appointment via the appointment ID
 - View a list of appointments filtered by patient name, clinician name, department, patientId, clinicianId and status. Multiple filters can be applied at once.
 - Change status of an appointment to Pending, Completed, Cancelled, EnteredInError

All the above are implemented using HTTP requests. Please see How to Run for specifics.

## Technical Highlights
 - Implementation of Entity Framework Core with DbContext and database constraints using the fluent API + conventions with a code first approach.
 - Asynchronous methods with async/await throughout data repository and service layers.
 - Output DTOs that have mapper extension methods that provide a patient and clinician summary object within an appointment DTO.
 - Dbseeder class that populates the clinician and patient tables if empty on start up.
 - Dependency injection using Scoped lifetimes for all objects, so created per Http request.
 - Implementation of a repository layer with interfaces.
 - Filter that allows for multiple filter parameters per query.
 - Migrations used as a workflow, can recreate schema from this repo if needed.
 - Inheritance structure used, with both patients and clinicians inheriting from a Person class that contains properties such as FirstName, LastName, Dob, Id, etc. A generic base class isn't conventional here but I wanted exposure to inheritance as a concept. In a production environemnt I would debate whether this structure is warranted.
 - CreateAppointmentStatus class implemented with a static factory for success, meaning appointment successfully created with the returned appointment, and failure, appointment failed to create with an error message returned instead.


## Design Decisions
 - Some null forgiving operators (!) were used throughout to silence compiler null warnings. This is due to the service layer accepting nullable value types on data that has already been validated in the controller layer. In the future separate command models could be implemented to push validation back into the controller layer.
 - A delete request is noticeably absent from the http methods implemented. This is because in a medical context you wouldn't want data being deleted entirely for auditing purposes. I decided instead have an EnteredInError status code to keep the implementation domain appropriate.
 - Further to the above point, restricted deletion has been added to foreign keys to further reinforce audit trail.
 - No controllers have been implemented for handling patient and clinicians yet. However, this is likely to come in a further project.
 - Self-implemented mappers have been used as extension methods as these allow for compile time errors, which wouldn't be the case if AutoMapper was used. 
 
## Status Codes
 - All successful get requests return a 200 Ok request. When these requests failed I generally returned a 400 Bad Request except the single appointment lookup returns a 404 not found. These seem the most appropriate and industry standard.
 - In my post request I return a 201 Created response when successful using the CreatedAtAction method that returns the location of the new resource along with its value in the response body. This returns a 400 Bad Request if the model state is invalid
 - For the patch request, I return a 404 not found if the appointment id isn't found and I return a 200 response if appointment status is successfully edited. This is so I can catch the case where the user has tried to change the status to the same status. The body of the 200 response will contain this message. 

## How to Run
Currently not deployed as a standalone application - so once you have pulled the repo run dotnet build then dotnet run. You will also need an SQL database and you will need to either add a connection string in the appsettings.json file or create an appsettings.Development.json file and add it in there. For example:
```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection" : "Server=COMPUTERNAME\\SQLEXPRESS;Database=DATABASENAME;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```
A user secret would be a more approriate approach, however it was considered out of scope for this project and will likely be added in subsequent projects.


## Endpoints

### List All Appointments
```
GET /api/appointment/listappointments
```
Response `200 OK`:
```json
[
    {
        "id": 1,
        "patient": {
            "id": 1,
            "name": "Kyle Murray"
        },
        "clinician": {
            "id": 2,
            "name": "William Murray",
            "specialty": "Cardiology"
        },
        "department": "Cardiology",
        "status": "Pending",
        "appointmentTime": "2026-04-15T09:00:00"
    }
]
```

---

### Get Appointment by ID
```
GET /api/appointment/getappointment/{id}
```
Response `200 OK`:
```json
{
    "id": 1,
    "patient": {
        "id": 1,
        "name": "Kyle Murray"
    },
    "clinician": {
        "id": 2,
        "name": "William Murray",
        "specialty": "Cardiology"
    },
    "department": "Cardiology",
    "status": "Pending",
    "appointmentTime": "2026-04-15T09:00:00"
}
```
Response `404 Not Found` - `"Appointment with id {id} not found"`

---

### Get Appointments by Filter
```
GET /api/appointment/getappointmentsby?patientId=1
GET /api/appointment/getappointmentsby?clinicianId=2
GET /api/appointment/getappointmentsby?department=Cardiology
GET /api/appointment/getappointmentsby?patientName=Murray
GET /api/appointment/getappointmentsby?clinicianName=William
GET /api/appointment/getappointmentsby?status=Pending
```
Multiple filters can be combined in a single request:
```
GET /api/appointment/getappointmentsby?department=Cardiology&status=Pending
```
Response `200 OK`:
```json
[
    {
        "id": 1,
        "patient": {
            "id": 1,
            "name": "Kyle Murray"
        },
        "clinician": {
            "id": 2,
            "name": "William Murray",
            "specialty": "Cardiology"
        },
        "department": "Cardiology",
        "status": "Pending",
        "appointmentTime": "2026-04-15T09:00:00"
    }
]
```
Response `400 Bad Request` - `"No query included"`

---

### Create Appointment
```
POST /api/appointment/createappointment
```
Request body:
```json
{
    "patientId": 1,
    "department": "Cardiology",
    "clinicianId": 2,
    "appointmentTime": "2026-04-15T09:00:00"
}
```
Response `201 Created`:
```json
{
    "id": 1,
    "patient": {
        "id": 1,
        "name": "Kyle Murray"
    },
    "clinician": {
        "id": 2,
        "name": "William Murray",
        "specialty": "Cardiology"
    },
    "department": "Cardiology",
    "status": "Pending",
    "appointmentTime": "2026-04-15T09:00:00"
}
```
Response `400 Bad Request` - Invalid or missing fields (e.g. `"Patient Id is required."`)  
Response `400 Bad Request` - `"Patient with this ID does not exist."`  
Response `400 Bad Request` - `"Clinician with this ID does not exist."`

---

### Change Appointment Status
```
PATCH /api/appointment/changeappointmentstatus
```
Request body:
```json
{
    "id": 1,
    "status": "Completed"
}
```
Response `200 OK` - `"Appointment status successfully changed to Completed"`  
Response `200 OK` - `"Appointment was already Completed. Appointment Status: Completed"`  
Response `404 Not Found` - `"Appointment not found"`

## Project Structure
```
├── Controllers/
│   └── AppointmentController.cs            # Handles HTTP requests
├── data/
│   └── ApiDbContext.cs                     # EF Core DbContext with fluent API configuration
├── DTOs/
│   ├── Output/
│   │   ├── ClinicianSummaryDTO.cs          # Nested clinician details for appointment responses
│   │   ├── PatientSummaryDTO.cs            # Nested patient details for appointment responses
│   │   └── ReturnAppointmentDTO.cs         # Appointment response shape with nested patient/clinician
│   ├── ChangeAppointmentStatusDTO.cs       # DTO for changing appointment status, includes ID and status
│   └── CreateAppointmentDTO.cs             # DTO for creating an appointment, excludes Id and status
├── Mappers/
│   ├── AppointmentOutputDTOMapper.cs       # Extension method mapping Appointment to ReturnAppointmentDTO
│   ├── ClinicianToClinicianSummaryMapper.cs # Extension method mapping Clinician to ClinicianSummaryDTO
│   └── PatientToPatientSummaryMapper.cs    # Extension method mapping Patient to PatientSummaryDTO
├── Migrations/                             # EF Core migration history for schema changes
├── Models/
│   ├── Appointment.cs                      # Appointment entity with navigation properties
│   ├── AppointmentStatus.cs                # Enum for appointment status codes
│   ├── Clinician.cs                        # Clinician entity inheriting from Person
│   ├── CreateAppointmentStatus.cs          # Result type for appointment creation with success/failure factories
│   ├── FilterParameters.cs                 # Container for multi-parameter filter queries
│   ├── Patient.cs                          # Patient entity inheriting from Person
│   └── Person.cs                           # Abstract base class for shared patient/clinician fields
├── Repositories/
│   ├── Implementations/
│   │   ├── AppointmentRepository.cs        # Appointment data access with EF Core
│   │   ├── ClinicianRepository.cs          # Clinician data access with EF Core
│   │   └── PatientRepository.cs            # Patient data access with EF Core
│   ├── IAppointmentRepository.cs           # Interface for appointment repository
│   ├── IClinicianRepository.cs             # Interface for clinician repository
│   └── IPatientRepository.cs               # Interface for patient repository
├── AppointmentService.cs                   # Service layer handling business logic
├── DbSeeder.cs                             # Seeds default patient and clinician data on startup
└── Program.cs                              # Program entry point and DI configuration
``` 