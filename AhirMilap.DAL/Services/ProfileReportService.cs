using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class ProfileReportService : IProfileReportService
    {
        private readonly IProfileReportRepository _repo;

        public ProfileReportService(IProfileReportRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult<ProfileReport>> AddReportUser(ProfileReport model)
        {
            var res = await _repo.AddReportUser(model);
            return AuthResult<ProfileReport>.TokenResult(res);
        }
        public async Task<AuthResult<ProfileReportResDTO>> GetReportUsers(Guid? profileFromId, string? userId)
        {
            var res = await _repo.GetReportUsers(profileFromId, userId);
            return AuthResult<ProfileReportResDTO>.Result(res);
        }

        public async Task<AuthResult<bool>> DeleteReportUserById(Guid? id)
        {
            var res = await _repo.DeleteReportUserById(id);
            return AuthResult<bool>.TokenResult(res);
        }

    }
}
