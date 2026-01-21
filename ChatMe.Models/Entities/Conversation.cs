using ChatMe.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.Entities
{
    public class Conversation
    {
        public string ConversationId { get; set; } = Guid.NewGuid().ToString();
        public string? ConversationName { get; set; } 
        public ConversationType Type { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<Message> Messages { get; set; }
        public ICollection<ConversationMember> Members { get; set; }
    }
}
