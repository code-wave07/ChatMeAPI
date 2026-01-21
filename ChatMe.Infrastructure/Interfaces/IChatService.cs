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

        // 7. Add a new member to a group chat
        Task AddMemberToGroupAsync(string adminUserId, AddMemberRequest request);

        // 8. Remove a group Member
        Task RemoveMemberFromGroupAsync(string adminUserId, RemoveMemberRequest request);

        // 9.Demote an Admin
        Task DemoteAdminToMemberAsync(string ownerUserId, DemoteMemberRequest request);

        // 10. Read receipts
        Task MarkMessagesAsReadAsync(string userId, string conversationId);

        // 11. Promote to Admin
        Task PromoteMemberToAdminAsync(string ownerUserId, PromoteMemberRequest request);

        // 12. Leave group
        Task LeaveGroupAsync(string userId, string conversationId);

        // 13. Update group info
        Task UpdateGroupInfoAsync(string adminUserId, string conversationId, string newName);

        // 14. Delete message
        Task DeleteMessageAsync(string userId, string messageId);
    }
}