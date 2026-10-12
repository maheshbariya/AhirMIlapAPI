using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class ConnectionService : IConnectionService
    {
        private readonly IConnectionRepository _repo;

        public ConnectionService(IConnectionRepository repo)
        {
            _repo = repo;
        }

        public async Task<AuthResult<Connection>> RequestConnection(Connection model)
        {
            var res = await _repo.RequestConnection(model);
            return AuthResult<Connection>.TokenResult(res);
        }
        public async Task<AuthResult<ConnectionResDTO>> GetConnectionUsers(Guid? profileId, int? actionId)
        {
            var res = await _repo.GetConnectionUsers(profileId, actionId);
            return AuthResult<ConnectionResDTO>.Result(res);
        }

        public async Task<AuthResult<bool>> ConnectionActionById(Guid? id, string? userId, int? actionId)
        {
            var res = await _repo.ConnectionActionById(id, userId,actionId);
            return AuthResult<bool>.TokenResult(res);
        }

    }
}
