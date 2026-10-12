using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IDashboardService
    {
        Task<AuthResult<DashboardProfilesBasicDetails>> GetProfilesList(Filter? filter,string? userId);
        Task<AuthResult<MatrimonyProfile>> GetProfileDetailsById(Guid? senderprofileId,Guid? profileId);
    }
}