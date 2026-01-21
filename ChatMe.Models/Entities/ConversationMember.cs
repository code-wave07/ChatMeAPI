using ChatMe.Models.Entities;
using ChatMe.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatMe.Models.Entities
{
    public class ConversationMember
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public DateTime JoinedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LeftDateTime { get; set; }
        public GroupRole Role { get; set; } = GroupRole.Member;
    }
}