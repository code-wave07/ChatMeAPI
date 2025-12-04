using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class AddMemberRequest
    {
        [Required]
        public string ConversationId { get; set; }

        [Required]
        public string NewMemberId { get; set; } 
    }
}
