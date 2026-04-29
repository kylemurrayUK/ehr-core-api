namespace EHRCoreAPI
{
    public class ClinicianRespository : IClinicianRepository
    {
        public ApiDbContext _db;
        public ClinicianRespository(ApiDbContext db)
        {
            _db = db;
        }
        public Clinician? GetClinician(int ClinicianId)
        {
            return _db.Clinicians.FirstOrDefault(c => c.Id == ClinicianId);
        }
    }
}