using Microsoft.EntityFrameworkCore;
using EHRCoreAPI.Models;
using EHRCoreAPI.Data;

namespace EHRCoreAPI.Repositories.Implementations
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