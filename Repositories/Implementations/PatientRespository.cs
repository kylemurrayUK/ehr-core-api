namespace EHRCoreAPI
{
    public class PatientRespository : IPatientRepository
    {
        public ApiDbContext _db;
        public PatientRespository(ApiDbContext db)
        {
            _db = db;
        }
        public Patient? GetPatient(int PatientId)
        {
            return _db.Patients.FirstOrDefault(p => p.Id == PatientId);
        }
    }
}