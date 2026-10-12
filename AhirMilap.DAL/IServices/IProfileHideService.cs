using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IProfileHideService
    {
        Task<AuthResult<ProfileHide>> AddHideUser(ProfileHide model);
        Task<AuthResult<ProfileHideResDTO>> GetHideUsers(Guid? profileFromId, string? userId);
        Task<AuthResult<bool>> DeleteHideUserById(Guid? id);
    }
}