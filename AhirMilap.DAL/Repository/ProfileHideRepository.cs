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
    public class ProfileHideRepository : IProfileHideRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GenerateDomainURL _generateDomainURL;
        public string connString = string.Empty;

        public ProfileHideRepository(IConfiguration configuration, GenerateDomainURL generateDomainURL)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
            _generateDomainURL = generateDomainURL;
        }

        public async Task<ProfileHide> AddHideUser(ProfileHide model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileIdFrom", model.ProfileIdFrom);
                dynamicParameters.Add("@ProfileIdTo", model.ProfileIdTo);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                var result = await connection.QueryAsync<ProfileHide>("dbo.[usp_AddHideUser]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<List<ProfileHideResDTO>> GetHideUsers(Guid? profileFromId, string? userId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileFromId", profileFromId);
                dynamicParameters.Add("@UserId", userId);
                var result = await connection.QueryAsync<ProfileHideResDTO>("dbo.[usp_GetHideUsers]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;

                return result.ToList();
            }
        }
        public async Task<bool> DeleteHideUserById(Guid? id)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", id);
                var result = await connection.QueryAsync<bool>("dbo.[usp_DeleteHideUserById]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return false;
                return result.FirstOrDefault();
            }
        }

    }
}
