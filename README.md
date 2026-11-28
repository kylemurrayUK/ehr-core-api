# ehr-core-api

## Overview
This is an EHR appointment management API that tracks appointments along with linking them to clinicians and patients. It is part of my C# backend development roadmap and has been built up across two projects:

- **Project 4** introduced an external SQL database using Entity Framework Core, along with a four-layer test suite.
- **Project 5** added authentication and authorisation: user accounts, password hashing, JWT login, role-based access and per-clinician data restrictions.

## Features
Users are able to
 - Log in with an email and password to receive a JWT access token
 - Create an appointment
 - View all appointments (clinicians only see their own)
 - View a specific appointment via the appointment ID
 - View a list of appointments filtered by patient name, clinician name, department, patientId, clinicianId and status. Multiple filters can be applied at once.
 - Change status of an appointment to Pending, Completed, Cancelled, EnteredInError, subject to role-based rules

All appointment endpoints require a valid token. Please see How to Run and Using Authentication for specifics.

## Technical Highlights
 - Implementation of Entity Framework Core with DbContext and database constraints using the fluent API + conventions with a code first approach.
 - Asynchronous methods with async/await throughout data repository and service layers.
 - Output DTOs that have mapper extension methods that provide a patient and clinician summary object within an appointment DTO.
 - DbSeeder class that populates the clinician, patient and user tables if empty on start up (Development environment only).
 - Dependency injection using Scoped lifetimes for objects that depend on the DbContext, and Singleton lifetimes for stateless services (password hashing, token generation, authorisation handlers).
 - Implementation of a repository layer with interfaces.
 - Filter that allows for multiple filter parameters per query.
 - Migrations used as a workflow, can recreate schema from this repo if needed.
 - Inheritance structure used, with both patients and clinicians inheriting from a Person class that contains properties such as FirstName, LastName, Dob, Id, etc. A generic base class isn't conventional here but I wanted exposure to inheritance as a concept. In a production environment I would debate whether this structure is warranted.
 - CreateAppointmentStatus class implemented with a static factory for success, meaning appointment successfully created with the returned appointment, and failure, appointment failed to create with an error message returned instead.
 - JWT bearer authentication with tokens issued by the API's own login endpoint and validated on every request.
 - Password hashing with PBKDF2 and per-user random salts, compared in constant time.
 - Role-based and resource-based authorisation using ASP.NET Core policies, custom requirements and authorisation handlers.
 - Strongly typed JWT configuration using the Options pattern, with the signing key held in user secrets rather than source control.

## How Authentication and Authorisation Work

### Accounts and passwords
Each user account has an email, a role (Admin, Clinician or Unassigned) and a nullable ClinicianId linking clinician users to their record in the Clinician table.

Passwords are never stored. When an account is created, a random 16-byte salt is generated and the password is **hashed** with PBKDF2 (SHA-256, 600,000 iterations) together with that salt. Only the resulting hash and the salt are stored. Hashing is one-way: the original password can't be recovered from it.

### Logging in (authentication)
A user logs in by sending their email and password to the login endpoint on the `AuthController`. The `AuthService` then:

1. Looks up the user by email (lowercased).
2. Re-hashes the submitted password with the user's stored salt, using the same PBKDF2 settings, and compares the result with the stored hash in constant time.
3. Refuses the login if the email doesn't exist or the hashes don't match (`401`), and refuses accounts that are not active (`403`): Unassigned users, and clinicians not linked to a clinician record.
4. Otherwise, asks the `TokenService` to issue a token.

### The token
The token is a **JSON Web Token (JWT)**. Its payload contains claims about the user:

- `sub` – the user's ID
- `email`
- the user's **role**
- `clinician_id` – a custom claim, only present for clinician users
- `jti` – a unique ID for this token
- `iss` / `aud` – the issuer and audience (this API)
- `exp` – the expiry time, 15 minutes after issue

The token is **signed, not encrypted**: anyone holding it can read the claims, so nothing sensitive is put in it. The signature is calculated over the header and payload using HMAC-SHA256 and a secret key, which in development is stored in user secrets. Anyone who changes a claim (for example, their role) breaks the signature, and without the key they can't create a valid new one.

The client sends the token in the `Authorization: Bearer <token>` header on every subsequent request.

### Each request (authentication, then authorisation)
1. **JwtBearer validates the token:** it recalculates the signature with the same key and checks the issuer, audience and expiry. If anything fails, or there's no token, the request is rejected with `401`. If it passes, the token's claims become the current user. No database lookup is needed, because the signature proves the server issued the token.
2. **Authorisation decides what that user may do,** using their role and claims: `[Authorize]` attributes, policies for status changes, and ownership checks using the `clinician_id` claim. A valid user who isn't allowed is rejected with `403` (or `404` when viewing another clinician's appointment; see Design Decisions).
3. Only then does the request reach the controller logic and return its response. For example, an Admin listing appointments receives all of them, while a Clinician receives only their own.

## Using Authentication

### Logging in
```
POST /api/Auth/Login
```
Request body:
```json
{
    "email": "adminemail@test.com",
    "password": "Test@678"
}
```
Responses:
- `200 OK` – login succeeded:
  ```json
  {
      "token": "eyJhbGciOiJIUzI1NiIs...",
      "expiresAt": "2026-11-24T18:15:00Z"
  }
  ```
- `400 Bad Request` – email or password missing
- `401 Unauthorized` – `"Invalid Email or Password"` (the same response whether the email or the password is wrong)
- `403 Forbidden` – `"Account is not active"` (the account has no role, or is a clinician account not linked to a clinician)

### Calling protected endpoints
All appointment endpoints require a token. Send it in the `Authorization` header:
```
Authorization: Bearer <token>
```
- No token, or an invalid/expired/tampered token → `401 Unauthorized`
- Valid token, but the user's role isn't allowed → `403 Forbidden`

Tokens expire after 15 minutes; log in again to get a new one.

### Development users
When running in the Development environment, the database seeder creates these users (they are never created in any other environment):

| Email | Password | Role | Notes |
|---|---|---|---|
| `adminemail@test.com` | `Test@678` | Admin | Can see all appointments |
| `clinicianemail@test.com` | `Clinic@89` | Clinician | Linked to clinician Johnny Doc (GMC 0987654) |
| `noroleemail@test.com` | `NoRole!890` | Unassigned | Login is refused with `403` – useful for demonstrating the fail-safe |

## Roles and Permissions

There are three roles. **Admin** represents administrative/booking staff, **Clinician** represents a clinician linked to a record in the Clinician table, and **Unassigned** is the default with no access.

| Action | Admin | Clinician | Unassigned |
|---|---|---|---|
| Log in | ✅ | ✅ (only if linked to a clinician) | ❌ `403` at login |
| List appointments | All appointments | **Own appointments only** | – |
| Get appointment by ID | Any appointment | Own only – others return `404` | – |
| Filter appointments | All matching | Own matching only | – |
| Create appointment | ✅ | ✅ | – |
| Set status to **Pending** | ✅ | ✅ | – |
| Set status to **Completed** | ❌ `403` | ✅ | – |
| Set status to **Cancelled** | ✅ | ❌ `403` | – |
| Set status to **EnteredInError** | ✅ | ✅ | – |

How this is enforced:
- **`[Authorize]`** on the appointment controller requires a valid token for every endpoint.
- **Status changes** use resource-based authorisation: a policy with a custom requirement and handler decides, per target status, which roles may set it (only someone who saw the patient can mark an appointment Completed; cancellations go through admin staff).
- **Viewing a single appointment** uses a second resource-based policy that compares the appointment's clinician with the `clinician_id` claim in the token.
- **Lists and filters** are restricted in the database query itself, so other clinicians' appointments are never loaded.

## Design Decisions

### General
 - A delete request is noticeably absent from the http methods implemented. This is because in a medical context you wouldn't want data being deleted entirely for auditing purposes. I decided instead have an EnteredInError status code to keep the implementation domain appropriate.
 - Further to the above point, restricted deletion has been added to foreign keys to further reinforce audit trail.
 - No controllers have been implemented for handling patient and clinicians yet. However, this is likely to come in a further project.
 - Self-implemented mappers have been used as extension methods as these allow for compile time errors, which wouldn't be the case if AutoMapper was used.

### Authentication and authorisation
 - There is a separate User table with a nullable ClinicianId, so that a user with the Clinician role can be linked to an existing row in the Clinician table. Login data stays separate from clinical data, and admins can exist without a clinician. Users with the Clinician role but no ClinicianId are refused at login.
 - User roles are stored as an enum with Unassigned as the first entry (value 0), which is what an unset role defaults to. Unassigned has no rights in the system, so if a role is ever forgotten, the result is no access rather than full access.
 - Inactive accounts are refused at login for the same reason: it's better to reject an inactive user than give it the benefit of the doubt. Refusing at login also means these accounts never hold a token at all.
 - A wrong email and a wrong password get the same response, so bad actors can't tell which piece of information they have is correct. The same logic applies to returning a 404 rather than a 403 when a clinician tries to view another clinician's appointment: a bad actor can't tell whether the appointment exists (protecting against insecure direct object references, IDOR).
 - Passwords are hashed with PBKDF2 (SHA-256, 600,000 iterations, a random 16-byte salt per user), a deliberately slow algorithm that makes brute-forcing expensive. Hashes are compared in constant time to avoid timing attacks.
 - Tokens are short-lived (15 minutes). JWTs can't be revoked once issued, so the lifetime limits the damage window if one is stolen.
 - One API both issues and validates tokens, so a single shared signing key (HS256) is used. The key is kept in user secrets, never in source control.
 - Clinicians' lists are filtered in the database query rather than loaded in full and checked afterwards: it's more efficient, and other clinicians' data is never in memory to leak.

## Status Codes
 - All successful get requests return a 200 Ok request. When these requests failed I generally returned a 400 Bad Request except the single appointment lookup returns a 404 not found. These seem the most appropriate and industry standard.
 - In my post request I return a 201 Created response when successful using the CreatedAtAction method that returns the location of the new resource along with its value in the response body. This returns a 400 Bad Request if the model state is invalid
 - For the patch request, I return a 404 not found if the appointment id isn't found and I return a 200 response if appointment status is successfully edited. This is so I can catch the case where the user has tried to change the status to the same status. The body of the 200 response will contain this message.
 - Any appointment endpoint called without a valid token returns a 401 Unauthorized.
 - A valid user attempting something their role doesn't allow (for example, an Admin marking an appointment Completed) receives a 403 Forbidden.
 - A clinician requesting another clinician's appointment receives exactly the same 404 response as for an appointment that doesn't exist, so the existence of the appointment isn't revealed.

## Testing
Testing was the key concept for the database stage of the project, with four layers implemented that move from complete isolation through to a live running instance. Each layer trades speed for realism. When authentication was added, every layer was updated to cover it.

### Unit Tests
Unit testing of the service layer implemented with xUnit and Moq. Repository interfaces are mocked so no database is involved, meaning these tests only prove the business logic itself is correct. The pass through methods that contain no logic were left out as they aren't appropriate for unit testing and are covered by the integration tests instead.

The authentication services are also unit tested:
 - The password hashing service (a correct password verifies, a near-miss is rejected, the same password hashed twice produces different salts and hashes).
 - The token service (the expected claims are present, the clinician claim only appears for linked clinicians, and the expiry falls within the configured window).
 - The auth service, with the user repository, hasher and token service mocked. This covers each login outcome, that the role is only checked after the password, that no token is generated on any failure path, and that emails are lowercased before lookup.

### Repository Integration Tests
Integration testing of the repository layer against a real SQL Server database, which is dropped, recreated and reseeded on each run. Mocking can't tell you whether the EF Core queries and mappings actually work, so a real database engine was used here.

This layer also covers user lookup by email and the database constraints on the User table: the unique email index and the filtered unique index on ClinicianId (multiple users without a clinician are allowed, but two users can't be linked to the same clinician).

### Controller Integration Tests
Integration testing of the controller layer using WebApplicationFactory, with the SQL Server dependency swapped out for an in-memory SQLite connection and requests sent through a real HttpClient. These run the controller, service and repository together, so they pick up wiring issues such as dependency injection, routing, model binding and status codes without needing a database instance running.

For authentication, the test host is given its own JWT settings, and tests create clients authenticated as a given role using the real `TokenService`. This means the real JwtBearer validation is exercised rather than bypassed. Tests cover 401s without a valid token, the full status permission matrix by role, the fail-safe for a clinician without a ClinicianId, and ownership: clinicians only see their own appointments, filtering by another clinician returns nothing, and another clinician's appointment returns an identical 404 to a missing one.

### Smoke Tests
Smoke testing with RestSharp against a genuinely running instance of the API over real HTTP. These are limited to a handful of critical path checks and are tagged with a Category trait so they can be run on their own. This is the only layer that proves the application actually boots and responds.

The smoke tests log in through the real login endpoint once per run and use the returned token for subsequent requests, as a real client would. Login itself is also smoke tested.

### Testing Design Decisions
 - The repository tests run against real SQL Server whilst the controller tests use in-memory SQLite. This is deliberate as the repository layer is where provider specific behaviour actually matters, whereas the controller tests are checking wiring rather than queries so the speed and zero setup of SQLite was deemed the better trade off there.
 - Controller tests generate real tokens rather than using a fake authentication handler, so that the real token validation configuration is tested.
 - Each repository test creates its own DbContext. Sharing a context between tests would let EF Core's change tracking fill in navigation properties or return cached entities, which could make tests pass for the wrong reason.

### How to Run Tests
Run all tests with dotnet test, or target a single layer by passing the test project path. Note that the repository integration tests require a local SQLEXPRESS instance, and the smoke tests require the API to already be running in the Development environment (so that the seeded users exist).

## How to Run
Currently not deployed as a standalone application - so once you have pulled the repo run dotnet build then dotnet run.

You will need an SQL database and you will need to either add a connection string in the appsettings.json file or create an appsettings.Development.json file and add it in there. For example:
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

You will also need to set a JWT signing key in user secrets. It isn't stored in the repository. From the `src/ehr-core-api` folder:
```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 32)"
```
The other JWT settings (issuer, audience and token lifetime) aren't secret and live in `appsettings.json` under the `Jwt` section.

The signing key uses user secrets, and the connection string could be moved there in the same way. Proper secret management for a deployed environment is planned for the next project.

## Known Limitations and Production Considerations
This is a learning project, and some things have been deliberately left out of scope:

- **Status changes are role-based only.** Clinicians are not restricted to their own appointments when changing status; ownership checks currently apply to viewing only.
- **No refresh tokens.** Users log in again every 15 minutes. Tokens can't be revoked before they expire, and a change to a user's role only takes effect at their next login.
- **No user management.** There are no endpoints for creating or editing users; users are seeded, and only in Development. Provisioning users in a deployed environment is still to be designed.
- **No rate limiting or account lockout on login,** so repeated password guessing isn't throttled.
- **Login timing:** a login for an unknown email returns faster than one with a wrong password (the password hash is only computed when the user exists), which could in theory reveal whether an account exists.
- **Smoke test credentials are hardcoded** to the development seed users (marked with a TODO). Against a deployed environment, they would come from environment variables or pipeline secrets.
- **Signing key storage:** user secrets are for local development only. A deployed instance would hold the key in its hosting configuration or a key vault.
- **In production,** an established identity solution would usually be preferred to hand-rolled authentication: for example ASP.NET Core Identity for user and password management, or an external identity provider (such as Microsoft Entra ID) issuing tokens via OAuth 2.0 / OpenID Connect. Systems with several services validating tokens would typically use asymmetric signing (RS256), so only the identity provider holds the private key.