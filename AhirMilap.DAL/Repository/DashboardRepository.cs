using AhirMilap.DAL.IRepository;
using AhirMilap.DTO;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhirMilap.Entity;
using AhirMilap.DAL.CommonService;

namespace AhirMilap.DAL.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GenerateDomainURL _generateDomainURL;
        public string connString = string.Empty;

        public DashboardRepository(IConfiguration configuration,GenerateDomainURL generateDomainURL)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
            _generateDomainURL = generateDomainURL;
        }
        public async Task<List<DashboardProfilesBasicDetails>> GetProfilesList(Filter? filter, string? userId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@LoginUserId", userId);
                dynamicParameters.Add("@rPageIndex", filter?.PageIndex);
                dynamicParameters.Add("@rPageSize", filter?.PageSize);
                dynamicParameters.Add("@rSearch", filter?.Search);
                dynamicParameters.Add("@rMinAge", filter?.MinAge);
                dynamicParameters.Add("@rMaxAge", filter?.MaxAge);
                dynamicParameters.Add("@rMinHeight", filter?.MinHeight);
                dynamicParameters.Add("@rMaxHeight", filter?.MaxHeight);
                dynamicParameters.Add("@rEducation", filter?.Education);
                dynamicParameters.Add("@rMaritalStatus", filter?.MaritalStatus);
                dynamicParameters.Add("@rProfession", filter?.Profession);
                dynamicParameters.Add("@rMinIncome", filter?.MinIncome);
                dynamicParameters.Add("@rMaxIncome", filter?.MaxIncome);
                dynamicParameters.Add("@rLocation", filter?.Location);
                dynamicParameters.Add("@rFilterId", filter?.FilterId);
                var result = (await connection.QueryAsync<DashboardProfilesBasicDetails>(
                    "[dbo].[usp_GetDashboardProfileList]",
                    dynamicParameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                if (!result.Any())
                    return null;

                result.ForEach(item =>
                {
                    item.ProfileFilePath = _generateDomainURL.GetFullUrl(item.ProfileFilePath);
                });

                return result;
            }
        }
        public async Task<MatrimonyProfile> GetProfileDetailsById(Guid? senderprofileId, Guid? profileId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@SenderProfileId", senderprofileId);
                dynamicParameters.Add("@Id", profileId);
                var reader = await connection.QueryMultipleAsync("dbo.[usp_GetMatrimonyProfilePublic]", dynamicParameters, commandType: CommandType.StoredProcedure);
                var res = reader.Read<MatrimonyProfile>().FirstOrDefault();
                if (res == null) return null; 
                var profiles = reader.Read<MatrimonyProfilePhotos>().ToList(); 
                res.ProfilePhotos = profiles;
                var siblings = reader.Read<Siblings>().ToList();
                res.Siblings = siblings;
                return res;
            }
        }
    }
}
