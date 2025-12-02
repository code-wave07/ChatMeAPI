using ChatMe.Models.Entities;
using ChatMe.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatMe.Models.Entities
{
    public class GroupMember
    {
        [Key]
        public string Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [ForeignKey("Conversation")]
        public string ConversationId { get; set; }
        public Conversation Conversation { get; set; }

        public DateTime JoinedDateTime { get; set; } = DateTime.UtcNow;
        public DateTime? LeftDateTime { get; set; }
        public GroupRole Role { get; set; } = GroupRole.Member;
    }
}