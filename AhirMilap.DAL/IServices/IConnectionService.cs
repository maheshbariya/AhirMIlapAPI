using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IConnectionService
    {
        Task<AuthResult<Connection>> RequestConnection(Connection model);
        Task<AuthResult<ConnectionResDTO>> GetConnectionUsers(Guid? profileId, int? actionId);
        Task<AuthResult<bool>> ConnectionActionById(Guid? id, string? userId,int? actionId);
    }
}
