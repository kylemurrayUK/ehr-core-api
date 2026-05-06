namespace EHRCoreAPI
{
    public class PatientRepository : IPatientRepository
    {
        public ApiDbContext _db;
        public PatientRepository(ApiDbContext db)
        {
            _db = db;
        }
        public Patient? GetPatient(int PatientId)
        {
            return _db.Patients.FirstOrDefault(p => p.Id == PatientId);
        }
    }
}