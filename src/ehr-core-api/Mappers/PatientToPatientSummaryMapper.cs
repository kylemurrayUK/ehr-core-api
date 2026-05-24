using EHRCoreAPI.Dtos.Output;
using EHRCoreAPI.Models;

namespace EHRCoreAPI.Mappers
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