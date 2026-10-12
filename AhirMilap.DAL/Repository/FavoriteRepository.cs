using AhirMilap.DAL.IRepository;
using AhirMilap.DTO;
using AhirMilap.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AhirMilap.DAL.Repository
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly IConfiguration _configuration;
        public string connString = string.Empty;

        public FavoriteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
        } 

        public async Task<FavoriteUser> AddFavoriteUser(FavoriteUser model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileIdFrom", model.ProfileIdFrom); 
                dynamicParameters.Add("@ProfileIdTo", model.ProfileIdTo); 
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                var result = await connection.QueryAsync<FavoriteUser>("dbo.[usp_AddFavoriteUser]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> DeleteFavoriteUserById(Guid? id)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", id);
                var result = await connection.QueryAsync<bool>("dbo.[usp_DeleteFavoriteUserById]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return false;
                return result.FirstOrDefault();
            }
        }

        public async Task<List<FavoiteResDTO>> GetFavoriteUsers(Guid? profileFromId, string? userId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileFromId", profileFromId);
                dynamicParameters.Add("@UserId", userId);
                var result = await connection.QueryAsync<FavoiteResDTO>("dbo.[usp_GetFavoriteUsers]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.ToList();
            }
        }
    }
}
