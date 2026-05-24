using EHRCoreAPI.Models;

namespace EHRCoreAPI.Repositories
{
    public interface IClinicianRepository
    {
        Task<Clinician?> GetClinicianAsync(int ClinicianId);
    }
}