namespace EHRCoreAPI
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientAsync(int PatientId);
    }
}