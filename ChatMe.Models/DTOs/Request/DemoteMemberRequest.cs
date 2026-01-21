using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class DemoteMemberRequest
    {
        [Required]
        public string ConversationId { get; set; }
        [Required]
        public string MemberIdToDemote { get; set; }
    }
}