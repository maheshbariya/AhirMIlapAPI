using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IProfileReportService
    {
        Task<AuthResult<ProfileReport>> AddReportUser(ProfileReport model);
        Task<AuthResult<ProfileReportResDTO>> GetReportUsers(Guid? profileFromId, string? userId);
        Task<AuthResult<bool>> DeleteReportUserById(Guid? id);
    }
}
