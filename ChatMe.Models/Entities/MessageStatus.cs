using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatMe.Models.Entities
{
    public class MessageStatus
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string MessageId { get; set; }
        public Message Message { get; set; }

        public string UserId { get; set; } // The person who READ it
        public ApplicationUser User { get; set; }

        public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    }
}
