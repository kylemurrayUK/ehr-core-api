namespace EHRCoreAPI
{
    public interface IPatientRepository
    {
        Patient? GetPatient(int patientId);
    }
}