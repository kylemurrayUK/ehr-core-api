namespace EHRCoreAPI
{
    public interface IClinicianRepository
    {
        Task<Clinician?> GetClinicianAsync(int ClinicianId);
    }
}