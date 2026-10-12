using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _repo;

        public FavoriteService(IFavoriteRepository repo)
        {
            _repo = repo;
        }   

        public async Task<AuthResult<FavoriteUser>> AddFavoriteUser(FavoriteUser model)
        {
            var res = await _repo.AddFavoriteUser(model);
            return AuthResult<FavoriteUser>.TokenResult(res);
        } 

        public async Task<AuthResult<bool>> DeleteFavoriteUserById(Guid? id)
        {
            var res = await _repo.DeleteFavoriteUserById(id);
            return AuthResult<bool>.TokenResult(res);
        }

        public async Task<AuthResult<FavoiteResDTO>> GetFavoriteUsers(Guid? profileFromId, string? userId)
        {
            var res = await _repo.GetFavoriteUsers(profileFromId, userId);
            return AuthResult<FavoiteResDTO>.Result(res);
        }
    }
}
