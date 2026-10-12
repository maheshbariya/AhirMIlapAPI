using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IAuthService
    {
        Task<AuthResult<int>> VerifyOTP(string email, int? otp); 
        Task<AuthResult<MatrimonyProfile>> AddMatrimonyProfile(MatrimonyProfile model); 
        Task<AuthResult<MatrimonyProfile>> GetMatrimonyProfile(string userId); 
        Task<AuthResult<CommonDrpResDTO>> GetStates(); 
        Task<AuthResult<CommonDrpResDTO>> GetDistrictByStateId(int stateId);
        Task<AuthResult<CommonDrpResDTO>> GetProfession();
        Task<AuthResult<bool>> DeleteProfilePhoto(Guid? id);
        Task<AuthResult<MatrimonyProfilePhotos>> UpdateProfilePhoto(MatrimonyProfilePhotos model);
    }
}
