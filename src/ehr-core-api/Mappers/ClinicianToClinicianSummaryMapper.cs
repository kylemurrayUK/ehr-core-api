using EHRCoreAPI.Dtos.Output;
using EHRCoreAPI.Models;

namespace EHRCoreAPI.Mappers
{
    public static class ClinicianToClinicianSummaryMapper
    {
        public static ClinicianSummaryDTO ToClinicianSummary(this Clinician clinician )
        {
            return new ClinicianSummaryDTO
            {
                Id = clinician.Id,
                Name = clinician.FullName,
                Specialty = clinician.Specialty
            };
        }
    }
}