namespace EHRCoreAPI
{
    public static class PatientToPatientSummaryMapper
    {
        public static PatientSummaryDTO ToPatientSummary(this Patient patient)
        {
            return new PatientSummaryDTO
            {
                Id = patient.Id,
                Name = patient.FullName,
            };
        }
    }
}