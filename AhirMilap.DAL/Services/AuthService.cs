using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;

        public AuthService(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        public async Task<AuthResult<int>> VerifyOTP(string email, int? otp)
        {
            var res = await _authRepo.VerifyOTP(email, otp);
            return AuthResult<int>.TokenResult(res);
        }

        public async Task<AuthResult<MatrimonyProfile>> AddMatrimonyProfile(MatrimonyProfile model)
        {
            var res = await _authRepo.AddMatrimonyProfile(model);
            return AuthResult<MatrimonyProfile>.TokenResult(res);
        }

        public async Task<AuthResult<MatrimonyProfile>> GetMatrimonyProfile(string userId)
        {
            var res = await _authRepo.GetMatrimonyProfile(userId);
            return AuthResult<MatrimonyProfile>.TokenResult(res);
        }

        public async Task<AuthResult<CommonDrpResDTO>> GetStates()
        {
            var res = await _authRepo.GetStates();
            return AuthResult<CommonDrpResDTO>.Result(res);
        }

        public async Task<AuthResult<CommonDrpResDTO>> GetDistrictByStateId(int stateId)
        {
            var res = await _authRepo.GetDistrictByStateId(stateId);
            return AuthResult<CommonDrpResDTO>.Result(res);
        }
        public async Task<AuthResult<CommonDrpResDTO>> GetProfession()
        {
            var res = await _authRepo.GetProfession();
            return AuthResult<CommonDrpResDTO>.Result(res);
        }

        public async Task<AuthResult<bool>> DeleteProfilePhoto(Guid? id)
        {
            var res = await _authRepo.DeleteProfilePhoto(id);
            return AuthResult<bool>.TokenResult(res);
        }

        public async Task<AuthResult<MatrimonyProfilePhotos>> UpdateProfilePhoto(MatrimonyProfilePhotos model)
        {
            var res = await _authRepo.UpdateProfilePhoto(model);
            return AuthResult<MatrimonyProfilePhotos>.TokenResult(res);
        }
    }
}
