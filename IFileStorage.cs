namespace EHRCoreAPI
{
    public interface IFileStorage
    {
        List<Appointment> LoadFile();
        void SaveFile(List<Appointment> appointments);

    }
}
