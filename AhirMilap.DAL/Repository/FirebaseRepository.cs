using AhirMilap.DAL.IRepository;
using AhirMilap.DTO;
using AhirMilap.Entity;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AhirMilap.DAL.Repository
{
    public class FirebaseRepository : IFirebaseRepository
    {
        private readonly IConfiguration _configuration;
        public string connString = string.Empty;

        public FirebaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            connString = ConfigurationExtensions.GetConnectionString(_configuration, "localDb");
        }

        public async Task<FirebaseToken> AddFirebaseToken(FirebaseToken model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@UserId", model.UserId);
                dynamicParameters.Add("@FcmToken", model.FcmToken);
                dynamicParameters.Add("@IsLogOut", model.IsLogOut);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                dynamicParameters.Add("@CreatedOn", DateTime.Now);
                var result = await connection.QueryAsync<FirebaseToken>("dbo.[usp_AddFirebaseToken]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<bool> DeleteFirebaseTokenByToken(string tokens)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@TokenCsv", tokens); 
                var result = await connection.QueryAsync<bool>("dbo.[usp_DeleteFirebaseTokenByToken]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return false;
                return result.FirstOrDefault();
            }
        }

        public async Task<List<FirebaseToken>> GetFirebaseToken(Guid? profileToId, Guid? profileFromId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileToId", profileToId);
                dynamicParameters.Add("@ProfileFromId", profileFromId); 
                var result = await connection.QueryAsync<FirebaseToken>("dbo.[usp_GetFirebaseTokenByUserId]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.ToList();
            }
        }

        public async Task<ChatMessageResDTO> GetMessages(long chatRoomId, int pageIndex, int pageSize, Guid? profileId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ChatRoomId", chatRoomId);
                dynamicParameters.Add("@PageIndex", pageIndex);
                dynamicParameters.Add("@PageSize", pageSize);
                dynamicParameters.Add("@LoginProfileId", profileId);
                var reader = await connection.QueryMultipleAsync("dbo.[usp_GetMessages]", dynamicParameters, commandType: CommandType.StoredProcedure);
                var res = reader.Read<ChatMessageResDTO>().FirstOrDefault();
                res.Messages = reader.Read<ChatMessageRes>().ToList();
                return res;
            }
        }

        public async Task<Messages> SaveMessages(Messages model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ChatRoomId", model.ChatRoomId);
                dynamicParameters.Add("@SenderProfileId", model.SenderProfileId);
                dynamicParameters.Add("@MessageText", model.MessageText);
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                var result = await connection.QueryAsync<Messages>("dbo.[usp_SendMessage]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<ChatRooms> CreateChatRoom(ChatRooms model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Profile1Id", model.Profile1Id);
                dynamicParameters.Add("@Profile2Id", model.Profile2Id); 
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);
                var result = await connection.QueryAsync<ChatRooms>("dbo.[usp_CreateChatRoom]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<ChatRooms> GetChatRoom(Guid? profile1Id, Guid? profile2Id)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Profile1Id", profile1Id);
                dynamicParameters.Add("@Profile2Id", profile2Id); 
                var result = await connection.QueryAsync<ChatRooms>("dbo.[usp_GetChatRoom]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<Notifications> SaveNotifications(Notifications model)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@Id", model.Id);
                dynamicParameters.Add("@SenderProfileId", model.SenderProfileId);
                dynamicParameters.Add("@ReceiverProfileId", model.ReceiverProfileId);
                dynamicParameters.Add("@NotificationType", model.NotificationType);
                dynamicParameters.Add("@Title", model.Title);
                dynamicParameters.Add("@Message", model.Message);  
                dynamicParameters.Add("@CreatedBy", model.CreatedBy);  
                dynamicParameters.Add("@CreatedOn", DateTime.Now);  
                var result = await connection.QueryAsync<Notifications>("dbo.[usp_SaveNotifications]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.FirstOrDefault();
            }
        }

        public async Task<NotificationResDTO> GetNotifications(GetNotificationReqDTO model, Guid? profileId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileId", profileId);
                dynamicParameters.Add("@PageIndex", model.PageIndex);
                dynamicParameters.Add("@PageSize", model.PageSize);
                dynamicParameters.Add("@Type", model.Type);
                var reader = await connection.QueryMultipleAsync("dbo.[usp_GetNotificationsByProfileId]", dynamicParameters, commandType: CommandType.StoredProcedure);
                var res = reader.Read<NotificationResDTO>().FirstOrDefault();
                res.Notifications = reader.Read<NotificationRes>().ToList();
                return res; 
            }
        }

        public async Task<List<ChatListResDTO>> GetMyChatList(Guid? profileId, int pageIndex, int pageSize, string? search)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileId", profileId);
                dynamicParameters.Add("@PageIndex", pageIndex);
                dynamicParameters.Add("@PageSize", pageSize);
                dynamicParameters.Add("@Search", search);
                var result = await connection.QueryAsync<ChatListResDTO>("dbo.[usp_GetMyChatList]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return null;
                return result.ToList();
            }
        }

        public async Task<int> GetMyChatUnReadCount(Guid? profileId)
        {
            using (var connection = new SqlConnection(connString))
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ProfileId", profileId); 
                var result = await connection.QueryAsync<int>("dbo.[usp_GetMyChatUnReadCount]", dynamicParameters, commandType: CommandType.StoredProcedure);
                if (result.Count() == 0) return 0;
                return result.FirstOrDefault();
            }
        }
    }
}
