using ChatMe.Infrastructure.Implementations;
using ChatMe.Infrastructure.Interfaces;
using ChatMe.Models.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ChatMe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // <--- THIS LOCKS THE DOOR. Only logged-in users enter.
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IFileService _fileService;

        public ChatController(IChatService chatService, IFileService fileService)
        {
            _chatService = chatService;
            _fileService = fileService;
        }

        // Helper to get the ID from the Token (Safety First!)
        private string GetCurrentUserId()
        {
            // This claim was set in AuthService: new Claim(ClaimTypes.NameIdentifier, user.Id)
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        // 1. GET MY INBOX
        [HttpGet("inbox")]
        public async Task<IActionResult> GetMyChats()
        {
            var userId = GetCurrentUserId(); // We don't ask the user "Who are you?". We know.
            var chats = await _chatService.GetUserConversationsAsync(userId);
            return Ok(chats);
        }

        // 2. SEARCH USERS
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var users = await _chatService.SearchUsersAsync(query);
            return Ok(users);
        }

        // 3. CREATE PRIVATE CHAT
        [HttpPost("create-private")]
        public async Task<IActionResult> CreatePrivateChat([FromBody] CreatePrivateChatRequest request)
        {
            var userId = GetCurrentUserId();
            try
            {
                var chatId = await _chatService.CreatePrivateChatAsync(userId, request.TargetUserId);
                return Ok(new { ConversationId = chatId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 4. CREATE GROUP CHAT
        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var userId = GetCurrentUserId();
            var chatId = await _chatService.CreateGroupChatAsync(userId, request);
            return Ok(new { ConversationId = chatId });
        }

        // 5. SEND MESSAGE
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetCurrentUserId();
            try
            {
                // Logic is in Service, but WE control the 'senderId' here
                var response = await _chatService.SendMessageAsync(request, userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 6. GET MESSAGE HISTORY
        [HttpGet("history/{conversationId}")]
        public async Task<IActionResult> GetHistory(string conversationId)
        {
            var userId = GetCurrentUserId();
            try
            {
                var messages = await _chatService.GetMessagesAsync(conversationId, userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 7. ADD GROUP MEMBER
        [HttpPost("group/add-member")]
        public async Task<IActionResult> AddMember([FromBody] AddMemberRequest request)
        {
            var adminUserId = GetCurrentUserId(); // The person clicking the button
            try
            {
                await _chatService.AddMemberToGroupAsync(adminUserId, request);
                return Ok(new { Message = "Member added successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 8. REMOVE GROUP MEMEBER
        [HttpPost("group/remove-member")]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveMemberRequest request)
        {
            var adminUserId = GetCurrentUserId();
            try
            {
                await _chatService.RemoveMemberFromGroupAsync(adminUserId, request);
                return Ok(new { Message = "Member removed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 9. Demote Admin
        [HttpPost("group/demote")]
        public async Task<IActionResult> DemoteAdmin([FromBody] DemoteMemberRequest request)
        {
            try
            {
                await _chatService.DemoteAdminToMemberAsync(GetCurrentUserId(), request);
                return Ok(new { Message = "Admin demoted to Member successfully." });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // 10. Upload image

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
            try
            {
                var imageUrl = await _fileService.SaveImageAsync(file);
                // The frontend will take this URL and put it into 'SendMessageRequest.MediaUrl'
                return Ok(new { Url = imageUrl });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("mark-read/{conversationId}")]
        public async Task<IActionResult> MarkRead(string conversationId)
        {
            await _chatService.MarkMessagesAsReadAsync(GetCurrentUserId(), conversationId);
            return Ok();
        }

        // 11. Promote to Admin

        [HttpPost("group/promote")]
        public async Task<IActionResult> PromoteToAdmin([FromBody] PromoteMemberRequest request)
        {
            try
            {
                await _chatService.PromoteMemberToAdminAsync(GetCurrentUserId(), request);
                return Ok(new { Message = "Member promoted to Admin successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 12. LEAVE GROUP
        [HttpPost("group/leave/{conversationId}")]
        public async Task<IActionResult> LeaveGroup(string conversationId)
        {
            // Helper method we defined earlier to get ID from token
            var userId = GetCurrentUserId();
            try
            {
                await _chatService.LeaveGroupAsync(userId, conversationId);
                return Ok(new { Message = "You have left the group." });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // 13. RENAME GROUP
        [HttpPut("group/update/{conversationId}")]
        public async Task<IActionResult> UpdateGroup(string conversationId, [FromQuery] string newName)
        {
            var userId = GetCurrentUserId();
            try
            {
                await _chatService.UpdateGroupInfoAsync(userId, conversationId, newName);
                return Ok(new { Message = "Group updated successfully." });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        // 14. DELETE MESSAGE
        [HttpDelete("message/{messageId}")]
        public async Task<IActionResult> DeleteMessage(string messageId)
        {
            var userId = GetCurrentUserId();
            try
            {
                await _chatService.DeleteMessageAsync(userId, messageId);
                return Ok(new { Message = "Message deleted." });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}
