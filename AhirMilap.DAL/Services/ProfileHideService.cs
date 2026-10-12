using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class ProfileHideService : IProfileHideService
    {
        private readonly IProfileHideRepository _repo;

        public ProfileHideService(IProfileHideRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult<ProfileHide>> AddHideUser(ProfileHide model)
        {
            var res = await _repo.AddHideUser(model);
            return AuthResult<ProfileHide>.TokenResult(res);
        }
        public async Task<AuthResult<ProfileHideResDTO>> GetHideUsers(Guid? profileFromId, string? userId)
        {
            var res = await _repo.GetHideUsers(profileFromId, userId);
            return AuthResult<ProfileHideResDTO>.Result(res);
        }

        public async Task<AuthResult<bool>> DeleteHideUserById(Guid? id)
        {
            var res = await _repo.DeleteHideUserById(id);
            return AuthResult<bool>.TokenResult(res);
        }

    }
}
