using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class ProfileBlockService : IProfileBlockService
    {
        private readonly IProfileBlockRepository _repo;

        public ProfileBlockService(IProfileBlockRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult<ProfileBlock>> AddBlockUser(ProfileBlock model)
        {
            var res = await _repo.AddBlockUser(model);
            return AuthResult<ProfileBlock>.TokenResult(res);
        }
        public async Task<AuthResult<ProfileBlockResDTO>> GetBlockUsers(Guid? profileFromId, string? userId)
        {
            var res = await _repo.GetBlockUsers(profileFromId, userId);
            return AuthResult<ProfileBlockResDTO>.Result(res);
        }

        public async Task<AuthResult<bool>> DeleteBlockUserById(Guid? id)
        {
            var res = await _repo.DeleteBlockUserById(id);
            return AuthResult<bool>.TokenResult(res);
        }

    }
}
