using ChatMe.Data.Interfaces;
using ChatMe.Infrastructure.Hubs;
using ChatMe.Infrastructure.Interfaces;
using ChatMe.Models.DTOs.Requests;
using ChatMe.Models.DTOs.Responses;
using ChatMe.Models.Entities;
using ChatMe.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatMe.Infrastructure.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        // 1. SEARCH USERS
        public async Task<IEnumerable<UserSearchResponse>> SearchUsersAsync(string query)
        {
            var allUsers = await _unitOfWork.GetDbContext().Set<ApplicationUser>()
                .Where(u => u.PhoneNumber.Contains(query) ||
                            u.FirstName.Contains(query) ||
                            u.LastName.Contains(query))
                .Select(u => new UserSearchResponse
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    ProfilePhotoUrl = u.ProfilePhotoUrl
                })
                .Take(20)
                .ToListAsync();

            return allUsers;
        }

        // 2. CREATE PRIVATE CHAT
        public async Task<string> CreatePrivateChatAsync(string userId, string targetUserId)
        {
            if (userId == targetUserId) throw new Exception("You cannot chat with yourself.");

            // FIX: Using GetRepository
            var existingChat = await _unitOfWork.GetRepository<Conversation>()
                .GetQueryable(c => c.Type == ConversationType.Private &&
                                   c.Members.Any(m => m.UserId == userId) &&
                                   c.Members.Any(m => m.UserId == targetUserId))
                .FirstOrDefaultAsync();

            if (existingChat != null)
            {
                return existingChat.ConversationId;
            }

            var newChat = new Conversation
            {
                Type = ConversationType.Private,
                ConversationName = null
            };

            await _unitOfWork.GetRepository<Conversation>().AddAsync(newChat);
            await _unitOfWork.SaveChangesAsync();

            var members = new List<GroupMember>
            {
                new GroupMember { ConversationId = newChat.ConversationId, UserId = userId, Role = GroupRole.Owner },
                new GroupMember { ConversationId = newChat.ConversationId, UserId = targetUserId, Role = GroupRole.Member }
            };

            await _unitOfWork.GetRepository<GroupMember>().AddRangeAsync(members);
            await _unitOfWork.SaveChangesAsync();

            return newChat.ConversationId;
        }

        // 3. CREATE GROUP CHAT
        public async Task<string> CreateGroupChatAsync(string adminUserId, CreateGroupRequest request)
        {
            var newGroup = new Conversation
            {
                Type = ConversationType.Group,
                ConversationName = request.GroupName
            };

            await _unitOfWork.GetRepository<Conversation>().AddAsync(newGroup);
            await _unitOfWork.SaveChangesAsync();

            var members = new List<GroupMember>
            {
                new GroupMember { ConversationId = newGroup.ConversationId, UserId = adminUserId, Role = GroupRole.Admin }
            };

            foreach (var memberId in request.MemberIds)
            {
                members.Add(new GroupMember
                {
                    ConversationId = newGroup.ConversationId,
                    UserId = memberId,
                    Role = GroupRole.Member
                });
            }

            await _unitOfWork.GetRepository<GroupMember>().AddRangeAsync(members);
            await _unitOfWork.SaveChangesAsync();

            return newGroup.ConversationId;
        }

        // 4. SEND MESSAGE
        public async Task<MessageResponse> SendMessageAsync(SendMessageRequest request, string senderId)
        {
            var isMember = await _unitOfWork.GetRepository<GroupMember>()
                .AnyAsync(m => m.ConversationId == request.ConversationId && m.UserId == senderId);

            if (!isMember) throw new Exception("You are not a member of this chat.");

            var message = new Message
            {
                ConversationId = request.ConversationId,
                SenderId = senderId,
                MessageText = request.Content,
                MessageType = request.MessageType,
                SentDateTime = DateTime.UtcNow
            };

            await _unitOfWork.GetRepository<Message>().AddAsync(message);

            var conversation = await _unitOfWork.GetRepository<Conversation>().GetByIdAsync(request.ConversationId);
            if (conversation != null)
            {
                conversation.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.GetRepository<Conversation>().Update(conversation);
            }

            await _unitOfWork.SaveChangesAsync();

            var sender = await _unitOfWork.GetRepository<ApplicationUser>().GetByIdAsync(senderId);

            var response = new MessageResponse
            {
                MessageId = message.MessageId,
                SenderId = senderId,
                SenderName = sender.FirstName + " " + sender.LastName,
                Content = message.MessageText,
                MessageType = message.MessageType,
                SentAt = message.SentDateTime,
                IsMine = false
            };

            await _hubContext.Clients.Group(request.ConversationId).SendAsync("ReceiveMessage", response);

            response.IsMine = true;
            return response;
        }

        // 5. GET MESSAGES
        public async Task<IEnumerable<MessageResponse>> GetMessagesAsync(string conversationId, string userId)
        {
            var membership = await _unitOfWork.GetRepository<GroupMember>()
                .GetSingleByAsync(m => m.ConversationId == conversationId && m.UserId == userId);

            if (membership == null) throw new Exception("You are not in this chat.");

            var messages = await _unitOfWork.GetRepository<Message>()
                .GetQueryable(m => m.ConversationId == conversationId &&
                                   m.SentDateTime >= membership.JoinedDateTime &&
                                   !m.IsDeletedForEveryone)
                .Include(m => m.Sender)
                .OrderBy(m => m.SentDateTime)
                .Select(m => new MessageResponse
                {
                    MessageId = m.MessageId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.FirstName,
                    Content = m.MessageText,
                    MessageType = m.MessageType,
                    SentAt = m.SentDateTime,
                    IsMine = m.SenderId == userId
                })
                .ToListAsync();

            return messages;
        }

        // 6. GET USER CONVERSATIONS
        public async Task<IEnumerable<ConversationResponse>> GetUserConversationsAsync(string userId)
        {
            var memberships = await _unitOfWork.GetRepository<GroupMember>()
                .GetQueryable(m => m.UserId == userId && m.LeftDateTime == null)
                .Include(m => m.Conversation)
                .ThenInclude(c => c.Members)
                .ThenInclude(mem => mem.User)
                .OrderByDescending(m => m.Conversation.UpdatedAt ?? m.Conversation.CreatedAt)
                .ToListAsync();

            var responseList = new List<ConversationResponse>();

            foreach (var mem in memberships)
            {
                var conv = mem.Conversation;
                string name = conv.ConversationName;
                string otherUserId = null;

                if (conv.Type == ConversationType.Private)
                {
                    var otherMember = conv.Members.FirstOrDefault(m => m.UserId != userId);
                    if (otherMember != null)
                    {
                        name = $"{otherMember.User.FirstName} {otherMember.User.LastName}";
                        otherUserId = otherMember.UserId;
                    }
                    else
                    {
                        name = "Unknown User";
                    }
                }

                responseList.Add(new ConversationResponse
                {
                    ConversationId = conv.ConversationId,
                    ConversationName = name,
                    IsGroup = conv.Type == ConversationType.Group,
                    LastMessageTime = conv.UpdatedAt ?? conv.CreatedAt,
                    OtherUserId = otherUserId
                });
            }

            return responseList;
        }

        // 7. ADD NEW GROUP MEMEBER
        public async Task AddMemberToGroupAsync(string adminUserId, AddMemberRequest request)
        {
            // 1. Get the Group and the Admin's membership status
            var group = await _unitOfWork.GetRepository<Conversation>()
                .GetQueryable(c => c.ConversationId == request.ConversationId)
                .Include(c => c.Members)
                .FirstOrDefaultAsync();

            if (group == null) throw new Exception("Group not found.");
            if (group.Type == ConversationType.Private) throw new Exception("Cannot add members to a private chat.");

            // 2. Check if the requester is an Admin
            var adminMember = group.Members.FirstOrDefault(m => m.UserId == adminUserId);
            if (adminMember == null || adminMember.Role == GroupRole.Member)
            {
                throw new Exception("Only Admins can add new members.");
            }

            // 3. Check if the new user is already in the group
            if (group.Members.Any(m => m.UserId == request.NewMemberId))
            {
                throw new Exception("User is already in the group.");
            }

            // 4. Add the new member
            var newMember = new GroupMember
            {
                ConversationId = request.ConversationId,
                UserId = request.NewMemberId,
                Role = GroupRole.Member,
                JoinedDateTime = DateTime.UtcNow // Smart History: They won't see messages before this time
            };

            await _unitOfWork.GetRepository<GroupMember>().AddAsync(newMember);
            await _unitOfWork.SaveChangesAsync();

            // Optional: Notify the group via SignalR that someone joined
            await _hubContext.Clients.Group(request.ConversationId).SendAsync("UserJoined", request.NewMemberId);
        }

        // 8. REMOVE GROUP MEMBER
        public async Task RemoveMemberFromGroupAsync(string adminUserId, RemoveMemberRequest request)
        {
            // 1. Get Group and Members
            var group = await _unitOfWork.GetRepository<Conversation>()
                .GetQueryable(c => c.ConversationId == request.ConversationId)
                .Include(c => c.Members)
                .FirstOrDefaultAsync();

            if (group == null) throw new Exception("Group not found.");
            if (group.Type == ConversationType.Private) throw new Exception("Cannot remove members from a private chat.");

            // 2. Identify the Admin (Requester) and the Victim (Target)
            var adminMember = group.Members.FirstOrDefault(m => m.UserId == adminUserId && m.LeftDateTime == null);
            var targetMember = group.Members.FirstOrDefault(m => m.UserId == request.MemberIdToRemove && m.LeftDateTime == null);

            if (adminMember == null) throw new Exception("You are not a member of this group.");
            if (targetMember == null) throw new Exception("Member not found or already left the group.");

            // 3. Permission Check
            // Rule: Only Admins or Owners can remove people.
            if (adminMember.Role == GroupRole.Member)
            {
                throw new Exception("Only Admins can remove members.");
            }

            // Rule: An Admin cannot kick the Owner.
            if (targetMember.Role == GroupRole.Owner)
            {
                throw new Exception("You cannot remove the Group Owner.");
            }

            // Rule: An Admin cannot kick another Admin (Optional rule, usually only Owners can kick Admins)
            if (adminMember.Role == GroupRole.Admin && targetMember.Role == GroupRole.Admin)
            {
                throw new Exception("Admins cannot remove other Admins. Only the Owner can do that.");
            }

            // 4. Soft Delete (Set LeftDateTime)
            targetMember.LeftDateTime = DateTime.UtcNow;

            _unitOfWork.GetRepository<GroupMember>().Update(targetMember);
            await _unitOfWork.SaveChangesAsync();

            // Optional: Notify the group via SignalR
            await _hubContext.Clients.Group(request.ConversationId).SendAsync("UserLeft", request.MemberIdToRemove);
        }
    }
}