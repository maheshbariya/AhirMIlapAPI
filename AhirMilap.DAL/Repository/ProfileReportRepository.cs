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
    public class ProfileReportRepository : IProfileReportRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GenerateDomainURL _generateDomainURL;
        public string connString = string.Empty;

        public ProfileReportRepository(IConfiguration configuration, GenerateDomainURL generateDomainURL)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
            _generateDomainURL = generateDomainURL;
        }

        public async Task<ProfileReport> AddReportUser(ProfileReport model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileIdFrom", model.ProfileIdFrom);
                dynamicParameters.Add("@ProfileIdTo", model.ProfileIdTo);
                dynamicParameters.Add("@Reason", model.Reason);
                dynamicParameters.Add("@Comments", model.Comments);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                var result = await connection.QueryAsync<ProfileReport>("dbo.[usp_AddReportUser]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<List<ProfileReportResDTO>> GetReportUsers(Guid? profileFromId, string? userId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileFromId", profileFromId);
                dynamicParameters.Add("@UserId", userId);
                var result = await connection.QueryAsync<ProfileReportResDTO>("dbo.[usp_GetReportUsers]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;

                return result.ToList();
            }
        }
        public async Task<bool> DeleteReportUserById(Guid? id)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", id);
                var result = await connection.QueryAsync<bool>("dbo.[usp_DeleteReportUserById]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return false;
                return result.FirstOrDefault();
            }
        }

    }
}
