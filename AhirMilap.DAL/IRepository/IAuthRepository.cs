using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IAuthRepository
    {
        Task<int> VerifyOTP(string email, int? otp); 
        Task<MatrimonyProfile> AddMatrimonyProfile(MatrimonyProfile model); 
        Task<MatrimonyProfile> GetMatrimonyProfile(string userId); 
        Task<List<CommonDrpResDTO>> GetStates(); 
        Task<List<CommonDrpResDTO>> GetDistrictByStateId(int stateId);
        Task<List<CommonDrpResDTO>> GetProfession();
        Task<bool> DeleteProfilePhoto(Guid? id);
        Task<MatrimonyProfilePhotos> UpdateProfilePhoto(MatrimonyProfilePhotos model);
    }
}
