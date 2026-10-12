using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IProfileBlockService
    {
        Task<AuthResult<ProfileBlock>> AddBlockUser(ProfileBlock model);
        Task<AuthResult<ProfileBlockResDTO>> GetBlockUsers(Guid? profileFromId, string? userId);
        Task<AuthResult<bool>> DeleteBlockUserById(Guid? id);
    }
}
