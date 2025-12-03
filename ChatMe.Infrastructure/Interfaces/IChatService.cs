using ChatMe.Models.DTOs.Requests;
using ChatMe.Models.DTOs.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatMe.Infrastructure.Interfaces
{
    public interface IChatService
    {
        // 1. INBOX: Get all chats for the current user
        Task<IEnumerable<ConversationResponse>> GetUserConversationsAsync(string userId);

        // 2. HISTORY: Get messages (Smart History: Only what I'm allowed to see)
        Task<IEnumerable<MessageResponse>> GetMessagesAsync(string conversationId, string userId);

        // 3. SENDING: Save to DB + Broadcast via SignalR
        Task<MessageResponse> SendMessageAsync(SendMessageRequest request, string senderId);

        // 4. CREATION: Start a DM (Checks if one exists first)
        Task<string> CreatePrivateChatAsync(string userId, string targetUserId);

        // 5. CREATION: Start a Group
        Task<string> CreateGroupChatAsync(string adminUserId, CreateGroupRequest request);

        // 6. Search for users by Name or Phone Number
        Task<IEnumerable<UserSearchResponse>> SearchUsersAsync(string query);
    }
}