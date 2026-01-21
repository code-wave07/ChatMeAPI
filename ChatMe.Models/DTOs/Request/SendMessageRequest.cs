using ChatMe.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class SendMessageRequest
    {
        [Required]
        public string ConversationId { get; set; } // Which chat is this for?

        public string MessageText { get; set; } // Text message
        public string? MediaUrl { get; set; } // Optional media URL

        public MessageType MessageType { get; set; } = MessageType.Text;
    }
}
