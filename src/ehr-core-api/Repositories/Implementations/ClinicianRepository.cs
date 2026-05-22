using Microsoft.EntityFrameworkCore;

namespace EHRCoreAPI
{
    public class ClinicianRepository : IClinicianRepository
    {
        public ApiDbContext _db;
        public ClinicianRepository(ApiDbContext db)
        {
            _db = db;
        }
        public async Task<Clinician?> GetClinicianAsync(int ClinicianId)
        {
            return await _db.Clinicians.FirstOrDefaultAsync(c => c.Id == ClinicianId);
        }
    }
}