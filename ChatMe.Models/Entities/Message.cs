using ChatMe.Models.Entities;
using ChatMe.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatMe.Models.Entities
{
    public class Message
    {
        public string MessageId { get; set; } = Guid.NewGuid().ToString();

        public string ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        // UPDATE: Identity uses String (Guid) IDs
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }

        public string? MessageText { get; set; }
        public MessageType MessageType { get; set; } = MessageType.Text;
        public string? MediaUrl { get; set; }
        public DateTime SentDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeletedForEveryone { get; set; } = false;
    }
}