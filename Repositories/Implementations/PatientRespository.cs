namespace EHRCoreAPI
{
    public class PatientRespository : IPatientRepository
    {
        public ApiDbContext _db;
        public Patient? GetPatient(int PatientId)
        {
            return _db.Patients.FirstOrDefault(p => p.Id == PatientId);
        }
    }
}