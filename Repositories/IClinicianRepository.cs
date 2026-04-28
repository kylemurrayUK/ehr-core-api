namespace EHRCoreAPI
{
    public interface IClinicianRepository
    {
        bool DoesClinicianExist(int clinicianId);
    }
}