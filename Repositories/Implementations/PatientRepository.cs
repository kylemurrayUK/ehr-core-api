using Microsoft.EntityFrameworkCore;

namespace EHRCoreAPI
{
    public class PatientRepository : IPatientRepository
    {
        public ApiDbContext _db;
        public PatientRepository(ApiDbContext db)
        {
            _db = db;
        }
        public async Task<Patient?> GetPatient(int PatientId)
        {
            return await _db.Patients.FirstOrDefaultAsync(p => p.Id == PatientId);
        }
    }
}