namespace EHRCoreAPI
{
    public class ClinicianRepository : IClinicianRepository
    {
        public ApiDbContext _db;
        public ClinicianRepository(ApiDbContext db)
        {
            _db = db;
        }
        public Clinician? GetClinician(int ClinicianId)
        {
            return _db.Clinicians.FirstOrDefault(c => c.Id == ClinicianId);
        }
    }
}