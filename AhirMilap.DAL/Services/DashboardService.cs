using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repo;
        public DashboardService(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult<DashboardProfilesBasicDetails>> GetProfilesList(Filter? filter,string? userId)
        {
            var res = await _repo.GetProfilesList(filter,userId);
            return AuthResult<DashboardProfilesBasicDetails>.Result(res);
        }
        public async Task<AuthResult<MatrimonyProfile>> GetProfileDetailsById(Guid? senderprofileId,Guid? profileId)
        {
            var res = await _repo.GetProfileDetailsById(senderprofileId,profileId);
            return AuthResult<MatrimonyProfile>.TokenResult(res);
        }
    }
}