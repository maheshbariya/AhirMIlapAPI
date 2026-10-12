using AhirMilap.DAL.CommonService;
using AhirMilap.DAL.IServices;
using AhirMilap.DTO;
using AhirMilap.Entity;
using AhirMilapAPI.CommonService;
using AhirMilapAPI.Setup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;

namespace AhirMilapAPI.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IFirebaseService _service;
        private readonly IHubContext<ChatHub> _hub;
        private readonly ILocalizationService _localizer;
        private readonly FirebaseNotificationService _firebaseService;

        public ChatController(IFirebaseService service, IHubContext<ChatHub> hub, FirebaseNotificationService firebaseNotService, ILocalizationService localizer)
        {
            _service = service;
            _hub = hub;
            _firebaseService = firebaseNotService;
            _localizer = localizer;
        }

        [HttpGet("getMyChatList/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetMyChatList(int pageIndex, int pageSize, string? search)
        { 
            var chats = await _service.GetMyChatList(User.GetProfileId(), pageIndex, pageSize, search);
            if (chats.Res != null)
                return Ok(new CommonResponse(Status.Success, "Data gated successfully.", chats.Res));

            return Conflict(new CommonResponse(Status.NotFound, "Data is not found."));
        }

        [HttpPost("createChatRoom")]
        public async Task<IActionResult> CreateChatRoom(CreateRoomReqDTO request)
        {
            ChatRooms newRoom = new ChatRooms
            {
                Profile1Id = request.Profile1Id,
                Profile2Id = request.Profile2Id,
                CreatedBy = User.GetUserId(),
            };

            var res = await _service.CreateChatRoom(newRoom);
            if (res.Data != null && res.Data.CreatedBy != "exists")
            {
                return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res.Data));
            }

            return Conflict(new CommonResponse(Status.Error, _localizer.Get("DataAlreadyExists")));
        }

        [HttpGet("getChatRoom/{profile1Id}/{profile2Id}")]
        public async Task<IActionResult> GetChatRoom(Guid? profile1Id, Guid? profile2Id)
        {
            var messages = await _service.GetChatRoom(profile1Id, profile2Id);
            if (messages.Data != null)
                return Ok(new CommonResponse(Status.Success, _localizer.Get("DataGatedSuccessfully"), messages.Data));

            return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(SendMessageReqDTO request)
        {
            string userId = User.GetUserId();
            Messages message = new Messages
            {
                ChatRoomId = request.ChatRoomId,
                SenderProfileId = request.SenderProfileId,
                MessageText = request.MessageText,
                CreatedBy = userId,
            };

            var res = await _service.SaveMessages(message);

            await _hub.Clients.Group(request.ChatRoomId.ToString())
                .SendAsync(
                    "ReceiveMessage",
                    request.SenderProfileId,
                    request.MessageText);

            Console.WriteLine($"MESSAGE SENT TO ROOM: {request.ChatRoomId}");

            //Send push notification
            Notifications notification = new Notifications()
            {
                SenderProfileId = request.SenderProfileId,
                ReceiverProfileId = request.ReceiverProfileId,
                NotificationType = "NEW_MESSAGE",
                Message = request.MessageText,
                CreatedBy = userId,
            };
            await _firebaseService.SendNotificationAsync(notification);

            return Ok(new CommonResponse(Status.Success, _localizer.Get("DataSavedSuccessfully"), res?.Data));
        }

        [HttpGet("getMessages/{chatRoomId}/{pageIndex}/{pageSize}")]
        public async Task<IActionResult> GetMessages(long chatRoomId, int pageIndex, int pageSize)
        {
            var messages = await _service.GetMessages(chatRoomId, pageIndex, pageSize, User.GetProfileId());
            if (messages.Data != null)
                return Ok(new CommonResponse(Status.Success, _localizer.Get("DataGatedSuccessfully"), messages.Data));

            return Conflict(new CommonResponse(Status.NotFound, _localizer.Get("NotFound")));
        }

        [HttpGet("getMyChatUnReadCount")]
        public async Task<IActionResult> GetMyChatUnReadCount()
        {
            var chats = await _service.GetMyChatUnReadCount(User.GetProfileId());
            if (chats.Data > -1)
                return Ok(new CommonResponse(Status.Success, "Data gated successfully.", chats.Data));

            return Conflict(new CommonResponse(Status.NotFound, "Data is not found."));
        }
    }
}
