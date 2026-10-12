using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IProfileLikeRepository
    {
        Task<ProfileLike> AddLikeUser(ProfileLike model);
        Task<List<ProfileLikeResDTO>> GetLikeUsers(Guid? profileFromId, string? userId);
        Task<bool> DeleteLikeUserById(Guid? id);
    }
}
