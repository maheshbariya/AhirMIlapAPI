using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IFavoriteService
    { 
        Task<AuthResult<FavoriteUser>> AddFavoriteUser(FavoriteUser model); 
        Task<AuthResult<FavoiteResDTO>> GetFavoriteUsers(Guid? profileFromId, string? userId);
        Task<AuthResult<bool>> DeleteFavoriteUserById(Guid? id);
    }
}
