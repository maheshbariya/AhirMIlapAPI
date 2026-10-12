using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IConnectionRepository
    {
        Task<Connection> RequestConnection(Connection model);
        Task<List<ConnectionResDTO>> GetConnectionUsers(Guid? profileId, int? actionId);
        Task<bool> ConnectionActionById(Guid? id, string? userId, int? actionId);
    }
}
