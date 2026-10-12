using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IProfileLikeService
    {
        Task<AuthResult<ProfileLike>> AddLikeUser(ProfileLike model);
        Task<AuthResult<ProfileLikeResDTO>> GetLikeUsers(Guid? profileFromId, string? userId);
        Task<AuthResult<bool>> DeleteLikeUserById(Guid? id);
    }
}
