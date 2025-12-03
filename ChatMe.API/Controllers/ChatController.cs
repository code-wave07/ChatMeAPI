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

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
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
    }
}
