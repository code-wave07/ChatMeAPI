using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatMe.Models.Entities
{
    public class MessageStatus
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("Message")]
        public string MessageId { get; set; }
        public Message Message { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; } // The person who READ it
        public ApplicationUser User { get; set; }

        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    }
}
