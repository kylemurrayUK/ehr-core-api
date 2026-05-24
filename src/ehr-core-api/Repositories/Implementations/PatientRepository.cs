using Microsoft.EntityFrameworkCore;
using EHRCoreAPI.Models;
using EHRCoreAPI.Data;

namespace EHRCoreAPI.Repositories.Implementations
{
    public class PatientRepository : IPatientRepository
    {
        public ApiDbContext _db;
        public PatientRepository(ApiDbContext db)
        {
            _db = db;
        }
        public async Task<Patient?> GetPatientAsync(int PatientId)
        {
            return await _db.Patients.FirstOrDefaultAsync(p => p.Id == PatientId);
        }
    }
}