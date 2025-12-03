using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class CreatePrivateChatRequest
    {
        [Required]
        public string TargetUserId { get; set; } // The ID of the person I want to DM
    }
}
