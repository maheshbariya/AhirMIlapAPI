using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IRepository;
using AhirMilap.DTO;
using AhirMilap.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.Json;
using static Dapper.SqlMapper;

namespace AhirMilap.DAL.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly GenerateDomainURL _generateDomainURL;
        public string connString = string.Empty;
        public string domainpath = string.Empty;

        public AuthRepository(IConfiguration configuration,GenerateDomainURL generateDomainURL)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
            _generateDomainURL = generateDomainURL;
        }
        public async Task<int> VerifyOTP(string email, int? otp)
        {
            try
            {
                using (var connection = new SqlConnection(connString))
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@Email", email);
                    dynamicParameters.Add("@OTP", otp);
                    dynamicParameters.Add("@CurrentDT", DateTime.Now);
                    var result = await connection.QueryAsync<int>("dbo.[usp_VerifyOTP]", dynamicParameters, commandType: CommandType.StoredProcedure);
                    if (result.Count() == 0) return 0;
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<MatrimonyProfile> AddMatrimonyProfile(MatrimonyProfile model)
        {
            using (var connection = new SqlConnection(connString))
            {
                var siblingsJson = model.Siblings != null && model.Siblings.Any() ? JsonSerializer.Serialize(model.Siblings) : null;
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", model.Id);
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@PageFlag", model.PageFlag);
                //page 1,2
                dynamicParameters.Add("@ProfileFor", model.ProfileFor);
                dynamicParameters.Add("@FirstName", model.FirstName);
                dynamicParameters.Add("@LastName", model.LastName);
                dynamicParameters.Add("@Gender", model.Gender);
                dynamicParameters.Add("@DOB", model.DOB);
                dynamicParameters.Add("@Age", model.Age);
                dynamicParameters.Add("@MaritalStatus", model.MaritalStatus);
                dynamicParameters.Add("@State", model.State);
                dynamicParameters.Add("@District", model.District);
                dynamicParameters.Add("@AboutMe", model.AboutMe);
                // Page 3
                dynamicParameters.Add("@Religion", model.Religion);
                dynamicParameters.Add("@Caste", model.Caste);
                dynamicParameters.Add("@SubCaste", model.SubCaste);
                dynamicParameters.Add("@LanguagesKnown", model.LanguageKnow);
                // Page 4
                dynamicParameters.Add("@AadhaarFilePath1", model.AadhaarFilePath1);
                dynamicParameters.Add("@AadhaarFilePath2", model.AadhaarFilePath2);
                dynamicParameters.Add("@ProfilePhoto", model.ProfileFilePath);
                // Page 5
                dynamicParameters.Add("@HighestEducation", model.HighestEducation);
                dynamicParameters.Add("@CollegeUniversity", model.CollegeUniversity);
                dynamicParameters.Add("@Occupation", model.Occupation);
                dynamicParameters.Add("@CompanyName", model.CompanyName);
                dynamicParameters.Add("@AnnualIncome", model.AnnualIncome);
                // Page 6
                dynamicParameters.Add("@HeightCm", model.HeightCm);
                dynamicParameters.Add("@WeightKg", model.WeightKg);
                dynamicParameters.Add("@Complexion", model.Complexion);
                dynamicParameters.Add("@PhysicalDisability", model.PhysicalDisability);
                dynamicParameters.Add("@DisabilityDetails", model.DisabilityDetails);
                // Page 7
                dynamicParameters.Add("@FatherName", model.FatherName);
                dynamicParameters.Add("@FatherOccupation", model.FatherOccupation);
                dynamicParameters.Add("@MotherName", model.MotherName);
                dynamicParameters.Add("@MotherOccupation", model.MotherOccupation);
                dynamicParameters.Add("@FamilyType", model.FamilyType);
                dynamicParameters.Add("@SiblingsJson", siblingsJson);
                // Common
                dynamicParameters.Add("@CreatedBy", model.UserId);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                var reader = await connection.QueryMultipleAsync("dbo.[usp_AddMatrimonyProfile]", dynamicParameters, commandType: CommandType.StoredProcedure);
                var res = reader.Read<MatrimonyProfile>().FirstOrDefault();
                if (res == null) return null;
                res.ProfileFilePath = _generateDomainURL.GetFullUrl(res.ProfileFilePath);
                res.AadhaarFilePath1 = _generateDomainURL.GetFullUrl(res.AadhaarFilePath1);
                res.AadhaarFilePath2 = _generateDomainURL.GetFullUrl(res.AadhaarFilePath2);
                var profiles = reader.Read<MatrimonyProfilePhotos>().ToList();
                profiles.ForEach(x =>
                {
                    x.PhotoPath = _generateDomainURL.GetFullUrl(x.PhotoPath);
                });
                res.Siblings = reader.Read<Siblings>().ToList();

                res.ProfilePhotos = profiles;
                return res;
            }
        }

        public async Task<MatrimonyProfile> GetMatrimonyProfile(string userId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", userId); 
                var reader = await connection.QueryMultipleAsync("dbo.[usp_GetMatrimonyProfile]", dynamicParameters, commandType: CommandType.StoredProcedure);
                var res = reader.Read<MatrimonyProfile>().FirstOrDefault(); 
                if (res == null) return null;
                res.ProfileFilePath = _generateDomainURL.GetFullUrl(res.ProfileFilePath);
                res.AadhaarFilePath1 = _generateDomainURL.GetFullUrl(res.AadhaarFilePath1);
                res.AadhaarFilePath2 = _generateDomainURL.GetFullUrl(res.AadhaarFilePath2);
                var profiles = reader.Read<MatrimonyProfilePhotos>().ToList();
                profiles.ForEach(x =>
                {
                    x.PhotoPath = _generateDomainURL.GetFullUrl(x.PhotoPath);
                });
                res.ProfilePhotos = profiles;
                var siblings = reader.Read<Siblings>().ToList();
                res.Siblings = siblings;
                return res; 
            }
        }

        public async Task<List<CommonDrpResDTO>> GetStates()
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Lang", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                var result = await connection.QueryAsync<CommonDrpResDTO>("dbo.[usp_GetStates]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.ToList();
            }
        }

        public async Task<List<CommonDrpResDTO>> GetDistrictByStateId(int stateId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@StateId", stateId);
                dynamicParameters.Add("@Lang",CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                var result = await connection.QueryAsync<CommonDrpResDTO>("dbo.[usp_GetDistrictByStateId]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.ToList();
            }
        }
        public async Task<List<CommonDrpResDTO>> GetProfession()
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                var result = await connection.QueryAsync<CommonDrpResDTO>("dbo.[usp_GetProfession]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.ToList();
            }
        }

        public async Task<bool> DeleteProfilePhoto(Guid? id)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", id); 
                var result = await connection.QueryAsync<bool>("dbo.[usp_DeleteProfilePhotoById]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return false;
                return result.FirstOrDefault();
            }
        }

        public async Task<MatrimonyProfilePhotos> UpdateProfilePhoto(MatrimonyProfilePhotos model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@PhotoPath", model.PhotoPath);
                dynamicParameters.Add("@SortOrder", model.SortOrder);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                var result = await connection.QueryAsync<MatrimonyProfilePhotos>("dbo.[usp_UpdateProfilePhoto]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }
    }
}
