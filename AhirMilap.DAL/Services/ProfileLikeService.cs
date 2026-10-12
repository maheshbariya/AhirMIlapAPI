using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class ProfileLikeService : IProfileLikeService
    {
        private readonly IProfileLikeRepository _repo;

        public ProfileLikeService(IProfileLikeRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult<ProfileLike>> AddLikeUser(ProfileLike model)
        {
            var res = await _repo.AddLikeUser(model);
            return AuthResult<ProfileLike>.TokenResult(res);
        }
        public async Task<AuthResult<ProfileLikeResDTO>> GetLikeUsers(Guid? profileFromId, string? userId)
        {
            var res = await _repo.GetLikeUsers(profileFromId, userId);
            return AuthResult<ProfileLikeResDTO>.Result(res);
        }

        public async Task<AuthResult<bool>> DeleteLikeUserById(Guid? id)
        {
            var res = await _repo.DeleteLikeUserById(id);
            return AuthResult<bool>.TokenResult(res);
        }

    }
}
