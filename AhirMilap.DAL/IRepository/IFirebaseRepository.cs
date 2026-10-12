using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.IRepository
{
    public interface IFirebaseRepository
    {
        Task<FirebaseToken> AddFirebaseToken(FirebaseToken firebaseToken);
        Task<List<FirebaseToken>> GetFirebaseToken(Guid? profileToId, Guid? profileFromId);
        Task<bool> DeleteFirebaseTokenByToken(string tokens);
        Task<List<ChatListResDTO>> GetMyChatList(Guid? profileId, int pageIndex, int pageSize, string? search);
        Task<Messages> SaveMessages(Messages model);
        Task<ChatMessageResDTO> GetMessages(long chatRoomId, int pageIndex, int pageSize, Guid? profileId);
        Task<ChatRooms> CreateChatRoom(ChatRooms model);
        Task<ChatRooms> GetChatRoom(Guid? profileId1, Guid? profileId2);
        Task<Notifications> SaveNotifications(Notifications model);
        Task<NotificationResDTO> GetNotifications(GetNotificationReqDTO model, Guid? profileId);
        Task<int> GetMyChatUnReadCount(Guid? profileId);
    }
}