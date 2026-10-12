using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IDashboardRepository
    {
        Task<List<DashboardProfilesBasicDetails>> GetProfilesList(Filter? filter, string? userId);
        Task<MatrimonyProfile> GetProfileDetailsById(Guid? senderprofileId, Guid? profileId);
    }
}