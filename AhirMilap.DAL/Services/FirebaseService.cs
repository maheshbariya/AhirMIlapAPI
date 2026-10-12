using AhirMilap.DAL.IRepository;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;

namespace AhirMilap.DAL.Services
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IFirebaseRepository _repo;

        public FirebaseService(IFirebaseRepository repo)
        {
            _repo = repo;
        }
         
        public async Task<AuthResult<FirebaseToken>> AddFirebaseToken(FirebaseToken model)
        {
            var res = await _repo.AddFirebaseToken(model);
            return AuthResult<FirebaseToken>.TokenResult(res);
        }

        public async Task<AuthResult<bool>> DeleteFirebaseTokenByToken(string tokens)
        {
            var res = await _repo.DeleteFirebaseTokenByToken(tokens);
            return AuthResult<bool>.TokenResult(res);
        }

        public async Task<AuthResult<FirebaseToken>> GetFirebaseToken(Guid? profileToId, Guid? profileFromId)
        {
            var res = await _repo.GetFirebaseToken(profileToId, profileFromId);
            return AuthResult<FirebaseToken>.Result(res);
        }

        public async Task<AuthResult<ChatMessageResDTO>> GetMessages(long chatRoomId, int pageIndex, int pageSize, Guid? profileId)
        {
            var res = await _repo.GetMessages(chatRoomId, pageIndex, pageSize, profileId);
            return AuthResult<ChatMessageResDTO>.TokenResult(res);
        }

        public async Task<AuthResult<Messages>> SaveMessages(Messages model)
        {
            var res = await _repo.SaveMessages(model);
            return AuthResult<Messages>.TokenResult(res);
        }

        public async Task<AuthResult<ChatRooms>> CreateChatRoom(ChatRooms model)
        {
            var res = await _repo.CreateChatRoom(model);
            return AuthResult<ChatRooms>.TokenResult(res);
        }

        public async Task<AuthResult<ChatRooms>> GetChatRoom(Guid? profileId1, Guid? profileId2)
        {
            var res = await _repo.GetChatRoom(profileId1, profileId2);
            return AuthResult<ChatRooms>.TokenResult(res);
        }

        public async Task<AuthResult<Notifications>> SaveNotifications(Notifications model)
        {
            var res = await _repo.SaveNotifications(model);
            return AuthResult<Notifications>.TokenResult(res);
        }

        public async Task<AuthResult<NotificationResDTO>> GetNotifications(GetNotificationReqDTO model, Guid? profileId)
        {
            var res = await _repo.GetNotifications(model, profileId);
            return AuthResult<NotificationResDTO>.TokenResult(res);
        }

        public async Task<AuthResult<ChatListResDTO>> GetMyChatList(Guid? profileId, int pageIndex, int pageSize, string? search)
        {
            var res = await _repo.GetMyChatList(profileId, pageIndex, pageSize, search);
            return AuthResult<ChatListResDTO>.Result(res);
        }

        public async Task<AuthResult<int>> GetMyChatUnReadCount(Guid? profileId)
        {
            var res = await _repo.GetMyChatUnReadCount(profileId);
            return AuthResult<int>.TokenResult(res);
        }
    }
}
