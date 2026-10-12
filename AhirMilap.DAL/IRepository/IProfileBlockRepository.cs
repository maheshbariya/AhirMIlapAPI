using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IProfileBlockRepository
    {
        Task<ProfileBlock> AddBlockUser(ProfileBlock model);
        Task<List<ProfileBlockResDTO>> GetBlockUsers(Guid? profileFromId, string? userId);
        Task<bool> DeleteBlockUserById(Guid? id);
    }
}
