using ChatMe.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.Entities
{
    public class Conversation
    {
        [Key]
        public string ConversationId { get; set; }
        public string? ConversationName { get; set; } // Null for Private chats
        public ConversationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Message> Messages { get; set; }
        public ICollection<GroupMember> Members { get; set; }
    }
}
