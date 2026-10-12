using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IProfileReportRepository
    {
        Task<ProfileReport> AddReportUser(ProfileReport model);
        Task<List<ProfileReportResDTO>> GetReportUsers(Guid? profileFromId, string? userId);
        Task<bool> DeleteReportUserById(Guid? id);
    }
}
