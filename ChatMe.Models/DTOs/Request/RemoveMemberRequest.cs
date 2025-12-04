using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class RemoveMemberRequest
    {
        [Required]
        public string ConversationId { get; set; }

        [Required]
        public string MemberIdToRemove { get; set; }
    }
}
