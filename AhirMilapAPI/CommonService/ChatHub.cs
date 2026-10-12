using Microsoft.AspNetCore.SignalR;

namespace AhirMilapAPI.CommonService
{
    public class ChatHub : Hub
    {
        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            Console.WriteLine($"JOINED ROOM: {roomId} - {Context.ConnectionId}");
        }

        //public async Task SendMessage(string roomId, string senderId, string message)
        //{
        //    await Clients.Group(roomId).SendAsync("ReceiveMessage", senderId, message);
        //}

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"CONNECTED : {Context.ConnectionId}"); 
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"DISCONNECTED : {Context.ConnectionId}"); 
            await base.OnDisconnectedAsync(exception);
        }
    }
}
