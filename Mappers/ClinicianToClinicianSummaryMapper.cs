namespace EHRCoreAPI
{
    public static class ClinicianToClinicianSummaryMapper
    {
        public static ClinicianSummaryDTO ToClinicianSummary(this Clinician clinician )
        {
            return new ClinicianSummaryDTO
            {
                Id = clinician.Id,
                Name = clinician.FullName,
                Speciality = clinician.Specialty
            };
        }
    }
}