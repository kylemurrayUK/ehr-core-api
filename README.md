# ehr-core-api

## Overview
Project 4 on my C# Backend development roadmap. An ASP.NET web api for managing appointments which is RESTful  with an external SQL database handled with Entity Framework Core. The project also includes patient and clinicians with seeding data that runs on start-up. There is also a repository layer implemented with interfaces for easier testing later in the roadmap. There is also basic implementation of asynchronousity.


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
 - Asynchronous methods
 - Output DTO's that have mapper extension classes for providing patient and clinician names for each appointment.
 - Dbseeder class for default data that checks for data and if database is empty inputs base data.
 - Dependency injection using Scoped lifetimes for all objects, so created per Http request.
 - Implementation of a repository layer with interfaces.
 - Filter that allows for multiple filter parameters per query.
 - Migrations used as a workflow, can recreate schema from this repo if needed.
 - Inheritance structure used, with both patients and clinicians inheriting from a Person class that contains properties such as Name, Id, etc.
 - CreateApppointment class implemented with a static factory for success, meaning appointment successfully created with the returned appointment, and failure, appointment failed to create with an error message returned instead.


## Design Decisions
Below are some design decisions that are worth explaining
 - Some ! were used throughout to silence compiler null warnings. This is due to the service layer accepting null value types on data that has already been validated in the controller layer. In the future separate command models could be implemented to push validation back into the controller layer.
 - A delete request is noticeably absent from the http methods implemented. This is because in a medical context you wouldn't want data being deleted entirely for auditing purposes. I decided instead have an EnteredInError status code to keep the implementation domain appropriate.
 - Further to the above point, restricted deletion has been added to the foreign key.
 - No controllers have been implemented for handling patient and clinicians yet. However, this is likely to come in a further project.
 - Self-implemented mappers have been used as extension methods as these allow for compile time errors, which wouldn't be the case if AutoMapper was used. 
 
A brief discussion on my thought process behind my choice of status codes
 - All successful get requests return a 200 Ok request. When these requests failed I generally returned a 400 Bad Request except the single appointment lookup returns a 404 not found. These seem the most appropriate and industry standard.
 - In my post request I return a 201 Created response when successful using the CreatedAtAction method that returns the location of the new resource along with its value in the response body. This returns a 400 Bad Request if the model state is invalid
 - For the patch request, I return a 404 not found if the appointment id isn't found and I return a 200 response if appointment status is successfully edited. This is so I can catch the case where the user has tried to change the status to the same status. The body of the 200 response will contain this message. 

## How to Run
Currently not deployed as a standalone application - so once you have pulled the repo run dotnet build then dotnet run.

## Endpoints

### List All Appointments
```
GET /api/appointment/listappointments
```
Response `200 OK`:
```json
[
    {
        "Id": 1,
        "Patient": "John Smith",
        "Clinician": "Dr Jones",
        "Department": "Cardiology",
        "AppointmentTime": "2026-04-15T09:00:00",
        "Status": "Pending"
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
    "Id": 1,
    "Patient": "John Smith",
    "Clinician": "Dr Jones",
    "Department": "Cardiology",
    "AppointmentTime": "2026-04-15T09:00:00",
    "Status": "Pending"
}
```
Response `404 Not Found`

---

### Get Appointments by Parameter
```
GET /api/appointment/getappointments?patient=John Smith
GET /api/appointment/getappointments?clinician=Dr Jones
GET /api/appointment/getappointments?department=Cardiology
```
Response `200 OK`:
```json
[
    {
        "Id": 1,
        "Patient": "John Smith",
        "Clinician": "Dr Jones",
        "Department": "Cardiology",
        "AppointmentTime": "2026-04-15T09:00:00",
        "Status": "Pending"
    }
]
```
Response `400 Bad Request` - `"No query included"`  
Response `400 Bad Request` - `"More than one query parameter not allowed"`

---

### Create Appointment
```
POST /api/appointment/createappointment
```
Request body:
```json
{
    "Patient": "John Smith",
    "Clinician": "Dr Jones",
    "Department": "Cardiology",
    "AppointmentTime": "2026-04-15T09:00:00"
}
```
Response `201 Created`:
```json
{
    "Id": 1,
    "Patient": "John Smith",
    "Clinician": "Dr Jones",
    "Department": "Cardiology",
    "AppointmentTime": "2026-04-15T09:00:00",
    "Status": "Pending"
}
```
Response `400 Bad Request` - Invalid or missing fields

---

### Change Appointment Status
```
PATCH /api/appointment/changeappointmentstatus
```
Request body:
```json
{
    "Id": 1,
    "Status": "Completed"
}
```
Response `200 OK` - `"Appointment status successfully changed to Completed"`  
Response `200 OK` - `"Appointment was already Completed. Appointment Status: Completed"`  
Response `404 Not Found` - `"Appointment not found"`

## Project Structure
```
├── Controllers/
│   └── AppointmentController.cs       # Handles HTTP requests
├── DTOs/
│   ├── ChangeAppointmentStatusDTO.cs  # DTO for changing appointment status,  just includes ID and status
│   └── CreateAppointmentDTO.cs        # DTO for creating appointment, excludes Id and appointment status
├── Models/
│   ├── Appointment.cs                 # Defines structure of appointments
│   └── AppointmentStatus.cs           # Enum for appointment status codes  
├── AppointmentService.cs              # Service layer object that handles business logic
├── FileStorage.cs                     # Handles Saving and loading data
├── IFileStorage.cs                    # Interface for Filestorage
└── Program.cs                         # Program entry point
``` 