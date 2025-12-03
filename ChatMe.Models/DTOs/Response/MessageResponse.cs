using ChatMe.Models.Enums;

namespace ChatMe.Models.DTOs.Responses;

public class MessageResponse
{
    public string MessageId { get; set; }
    public string SenderId { get; set; }
    public string SenderName { get; set; } // "John Doe" (So UI doesn't have to look it up)
    public string Content { get; set; }
    public MessageType MessageType { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsMine { get; set; } // Helper for UI (Right vs Left side alignment)
}