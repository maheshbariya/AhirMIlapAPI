using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IFavoriteRepository
    {
        Task<FavoriteUser> AddFavoriteUser(FavoriteUser model);
        Task<List<FavoiteResDTO>> GetFavoriteUsers(Guid? profileFromId, string? userId);
        Task<bool> DeleteFavoriteUserById(Guid? id);
    }
}