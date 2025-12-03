using Microsoft.AspNetCore.SignalR;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChatMe.Infrastructure.Hubs
{
    // We can define a strong interface for the client to avoid "magic strings"
    // Clients will listen for: "ReceiveMessage", "UserJoined", etc.
    public class ChatHub : Hub
    {
        // 1. Join a specific chat room (Conversation)
        public async Task JoinChat(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
            // Optional: Notify others
            // await Clients.Group(conversationId).SendAsync("UserJoined", Context.ConnectionId);
        }

        // 2. Leave a chat room
        public async Task LeaveChat(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
        }

        // 3. Send Message (This is usually called by the API, not directly by the client, 
        //    but we keep it here just in case you want direct socket sending)
        public async Task SendMessage(string conversationId, object messageData)
        {
            await Clients.Group(conversationId).SendAsync("ReceiveMessage", messageData);
        }
    }
}