using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatMe.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        // 1. Join Chat (Connection enters the Room)
        public async Task JoinChat(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        }

        // 2. Leave Chat
        public async Task LeaveChat(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        // 3. Send Message (Usually called by API, but works here too)
        public async Task SendMessage(string conversationId, object messageData) 
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", messageData);
        }


        public async Task Typing(string conversationId, string userName)
        {
            // 'OthersInGroup' means everyone EXCEPT the sender.
            // Perfect for 1-on-1 (the other person sees it) AND Groups (everyone else sees it).
            await Clients.OthersInGroup(conversationId).SendAsync("UserTyping", conversationId, userName);
        }

        public async Task StoppedTyping(string conversationId, string userName)
        {
            await Clients.OthersInGroup(conversationId).SendAsync("UserStoppedTyping", conversationId, userName);
        }
    }
}