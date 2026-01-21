using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class PromoteMemberRequest
    {
        [Required]
        public string ConversationId { get; set; }

        [Required]
        public string MemberIdToPromote { get; set; }
    }
}
