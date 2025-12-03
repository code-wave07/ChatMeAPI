using System.ComponentModel.DataAnnotations;

namespace ChatMe.Models.DTOs.Requests
{
    public class CreateGroupRequest
    {
        [Required]
        public string GroupName { get; set; }

        [Required]
        public List<string> MemberIds { get; set; } // List of User IDs to add immediately
    }
}
