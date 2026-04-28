namespace EHRCoreAPI
{
    public interface IClinicianRepository
    {
        Clinician? GetClinician(int clinicianId);
    }
}