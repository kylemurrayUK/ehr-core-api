using EHRCoreAPI.Models;

namespace EHRCoreAPI.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientAsync(int PatientId);
    }
}