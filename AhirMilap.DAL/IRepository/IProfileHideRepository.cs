using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IProfileHideRepository
    {
        Task<ProfileHide> AddHideUser(ProfileHide model);
        Task<List<ProfileHideResDTO>> GetHideUsers(Guid? profileFromId, string? userId);
        Task<bool> DeleteHideUserById(Guid? id);
    }
}
