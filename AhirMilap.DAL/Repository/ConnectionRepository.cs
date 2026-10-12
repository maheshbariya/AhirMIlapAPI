using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IRepository;
using AhirMilap.DTO;
using AhirMilap.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AhirMilap.DAL.Repository
{
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GenerateDomainURL _generateDomainURL;
        public string connString = string.Empty;

        public ConnectionRepository(IConfiguration configuration, GenerateDomainURL generateDomainURL)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
            _generateDomainURL = generateDomainURL;
        }

        public async Task<Connection> RequestConnection(Connection model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileIdFrom", model.ProfileIdFrom);
                dynamicParameters.Add("@ProfileIdTo", model.ProfileIdTo);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                dynamicParameters.Add("@ModifiedBy", model.UpdatedBy);
                dynamicParameters.Add("@ModifiedOn", DateTime.Now);
                var result = await connection.QueryAsync<Connection>("[dbo].[usp_SendRequestConnectionUser]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<List<ConnectionResDTO>> GetConnectionUsers(Guid? profileId, int? actionId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileId", profileId);
                dynamicParameters.Add("@ActionId", actionId);
                var result = await connection.QueryAsync<ConnectionResDTO>("dbo.[usp_GetConnectionUsers]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;

                return result.ToList();
            }
        }
        public async Task<bool> ConnectionActionById(Guid? id, string? userId, int? actionId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", id);
                dynamicParameters.Add("@ActionId", actionId);
                dynamicParameters.Add("@ModifiedOn", DateTime.Now);
                dynamicParameters.Add("@ModifiedBy", userId);
                var result = await connection.QueryAsync<bool>("[dbo].[usp_RequestConnectionActionById]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return false;
                return result.FirstOrDefault();
            }
        }

    }
}
