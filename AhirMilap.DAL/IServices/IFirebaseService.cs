using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IServices
{
    public interface IFirebaseService
    {
        Task<AuthResult<FirebaseToken>> AddFirebaseToken(FirebaseToken firebaseToken); 
        Task<AuthResult<FirebaseToken>> GetFirebaseToken(Guid? profileToId, Guid? profileFromId); 
        Task<AuthResult<bool>> DeleteFirebaseTokenByToken(string tokens); 
        Task<AuthResult<ChatListResDTO>> GetMyChatList(Guid? profileId, int pageIndex, int pageSize, string? search); 
        Task<AuthResult<Messages>> SaveMessages(Messages model); 
        Task<AuthResult<ChatMessageResDTO>> GetMessages(long chatRoomId, int pageIndex, int pageSize, Guid? profileId); 
        Task<AuthResult<ChatRooms>> CreateChatRoom(ChatRooms model); 
        Task<AuthResult<ChatRooms>> GetChatRoom(Guid? profileId1, Guid? profileId2); 
        Task<AuthResult<Notifications>> SaveNotifications(Notifications model); 
        Task<AuthResult<NotificationResDTO>> GetNotifications(GetNotificationReqDTO model, Guid? profileId); 
        Task<AuthResult<int>> GetMyChatUnReadCount(Guid? profileId); 
    }
}
